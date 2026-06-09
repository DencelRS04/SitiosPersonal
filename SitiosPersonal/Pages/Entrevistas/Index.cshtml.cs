using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Entrevistas
{
    public class IndexModel : PageModel
    {
        private readonly EntrevistasService _repository;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosRepository;

        public IndexModel(
            EntrevistasService repository,
            BitacoraService bitacoraService,
            PermisosService permisosRepository)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
            _permisosRepository = permisosRepository;
        }

        public EntrevistasListaViewModel Lista { get; set; } = new EntrevistasListaViewModel();

        public EntrevistasListaViewModel ViewModel => Lista;

        public IActionResult OnGet(int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int cantidadPorPagina = 10;
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            Lista = new EntrevistasListaViewModel
            {
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _repository.Contar(),
                Entrevistas = _repository.ListarPaginado(pagina, cantidadPorPagina)
            };

            _bitacoraService.RegistrarConsulta(idUsuario, "Entrevistas");

            return Page();
        }

        public IActionResult OnPostMarcarRealizada(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var entrevista = _repository.ObtenerPorId(id);

            if (entrevista == null)
            {
                return RedirectToPage("Index");
            }

            _repository.MarcarRealizada(id);

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Entrevista",
                new { entrevista.id_entrevista, entrevista.id_oferente, estado = entrevista.estado },
                new { entrevista.id_entrevista, entrevista.id_oferente, estado = "REALIZADA" }
            );

            TempData["Exito"] = "Entrevista marcada como realizada correctamente.";
            return RedirectToPage("Index");
        }

        public IActionResult OnPostEliminar(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var entrevista = _repository.ObtenerPorId(id);

            if (entrevista == null)
            {
                return RedirectToPage("Index");
            }

            if (!_repository.PuedeEliminar(id))
            {
                TempData["Error"] = "No se puede eliminar un registro con datos relacionados.";
                return RedirectToPage("Index");
            }

            _repository.Eliminar(id);

            _bitacoraService.RegistrarDelete(
                idUsuario,
                "Entrevista",
                new { entrevista.id_entrevista, entrevista.id_oferente, entrevista.id_empleado_entrevistador, entrevista.estado }
            );

            TempData["Exito"] = "Entrevista eliminada correctamente.";
            return RedirectToPage("Index");
        }

        private IActionResult? ValidarAcceso()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                TempData["Mensaje"] = Request.Cookies.ContainsKey("SesionIniciada")
                    ? "La sesión ha expirado. Por favor inicie sesión nuevamente."
                    : "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var rutasPermitidas = _permisosRepository.ObtenerRutasPermitidas(idUsuario.Value);
            bool tienePermiso = rutasPermitidas.Any(ruta =>
                !string.IsNullOrWhiteSpace(ruta)
                && Request.Path.Value!.StartsWith(ruta, StringComparison.OrdinalIgnoreCase));

            if (!tienePermiso)
            {
                TempData["Error"] = "No tiene permisos para acceder a esta pantalla.";
                return RedirectToPage("/Home/Index");
            }

            return null;
        }
    }
}
