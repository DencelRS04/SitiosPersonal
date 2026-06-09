using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Areas
{
    public class IndexModel : PageModel
    {
        private readonly AreasService _areasService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosService;
        private const int CantidadPorPagina = 10;

        public AreasListaViewModel Lista { get; set; } = new();

        public AreasListaViewModel ViewModel => Lista;

        public IndexModel(AreasService areasService, BitacoraService bitacoraService, PermisosService permisosService)
        {
            _areasService = areasService;
            _bitacoraService = bitacoraService;
            _permisosService = permisosService;
        }

        public IActionResult OnGet(int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            Lista = _areasService.ObtenerPaginado(pagina, CantidadPorPagina);

            _bitacoraService.RegistrarConsulta(idUsuario, "Area");

            return Page();
        }

        public IActionResult OnPostEliminar(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var (exito, error) = _areasService.Eliminar(id);

            if (!exito)
            {
                TempData["Error"] = error;
            }
            else
            {
                _bitacoraService.RegistrarDelete(idUsuario, "Area", new { id_area = id });
                TempData["Exito"] = "Área eliminada correctamente.";
            }

            return RedirectToPage();
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

            if (_permisosService.EsAdministrador(idUsuario.Value))
            {
                return null;
            }

            var rutasPermitidas = _permisosService.ObtenerRutasPermitidas(idUsuario.Value);
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
