using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.Parametros
{
    public class IndexModel : PageModel
    {
        private readonly ParametroService _repository;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosRepository;

        public IndexModel(ParametroService repository, BitacoraService bitacoraService, PermisosService permisosRepository)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
            _permisosRepository = permisosRepository;
        }

        public ParametrosListaViewModel Lista { get; set; } = new ParametrosListaViewModel();

        public IActionResult OnGet(int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int cantidadPorPagina = 10;
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            Lista = new ParametrosListaViewModel
            {
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _repository.Contar(),
                Parametros = _repository.ListarPaginado(pagina, cantidadPorPagina)
            };

            _bitacoraService.RegistrarConsulta(idUsuario, "Parámetros");
            return Page();
        }

        public IActionResult OnPostEliminar(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var parametro = _repository.ObtenerPorId(id);

            if (parametro == null) return RedirectToPage("Index");

            if (!_repository.PuedeEliminar(id))
            {
                TempData["Error"] = "No se puede eliminar un registro con datos relacionados.";
                return RedirectToPage("Index");
            }

            _repository.Eliminar(id);
            _bitacoraService.RegistrarDelete(idUsuario, "Parámetro", new { parametro.id_parametro, parametro.codigo });
            TempData["Exito"] = "Parámetro eliminado correctamente.";
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
                !string.IsNullOrWhiteSpace(ruta) &&
                Request.Path.Value!.StartsWith(ruta, StringComparison.OrdinalIgnoreCase));

            if (!tienePermiso)
            {
                TempData["Error"] = "No tiene permisos para acceder a esta pantalla.";
                return RedirectToPage("/Home/Index");
            }

            return null;
        }
    }
}
