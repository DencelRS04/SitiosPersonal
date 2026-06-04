using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Concursos
{
    public class IndexModel : PageModel
    {
        private readonly ConcursosRepository _repository;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosRepository _permisosRepository;

        public IndexModel(
            ConcursosRepository repository,
            BitacoraService bitacoraService,
            PermisosRepository permisosRepository)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
            _permisosRepository = permisosRepository;
        }

        public ConcursosListaViewModel Lista { get; set; } = new ConcursosListaViewModel();

        public IActionResult OnGet(int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int cantidadPorPagina = 10;
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            Lista = new ConcursosListaViewModel
            {
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _repository.Contar(),
                Concursos = _repository.ListarPaginado(pagina, cantidadPorPagina)
            };

            _bitacoraService.RegistrarConsulta(idUsuario, "Concursos");

            return Page();
        }

        public IActionResult OnPostCambiarEstado(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var concurso = _repository.ObtenerPorId(id);

            if (concurso == null)
            {
                return RedirectToPage("Index");
            }

            string estadoAnterior = concurso.estado;
            _repository.CambiarEstado(id, estadoAnterior);

            string estadoNuevo = estadoAnterior == "VIGENTE" ? "VENCIDO" : "VIGENTE";

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Concurso",
                new { concurso.id_concurso, concurso.codigo, estado = estadoAnterior },
                new { concurso.id_concurso, concurso.codigo, estado = estadoNuevo }
            );

            TempData["Exito"] = $"El concurso ha sido marcado como {estadoNuevo}.";
            return RedirectToPage("Index");
        }

        public IActionResult OnPostEliminar(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var concurso = _repository.ObtenerPorId(id);

            if (concurso == null)
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
                "Concurso",
                new { concurso.id_concurso, concurso.codigo, concurso.nombre }
            );

            TempData["Exito"] = "Concurso eliminado correctamente.";
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
