using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Modulos
{
    public class IndexModel : PageModel
    {
        private readonly PantallasService _pantallasService;
        private readonly PermisosService _permisosService;

        public IndexModel(
            PantallasService pantallasService,
            PermisosService permisosService)
        {
            _pantallasService = pantallasService;
            _permisosService = permisosService;
        }

        public PantallasListaViewModel Lista { get; set; } = new PantallasListaViewModel();

        public PantallasListaViewModel ViewModel => Lista;

        public IActionResult OnGet(int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            Lista = _pantallasService.ObtenerListado(pagina, 10, idUsuario);

            return Page();
        }

        public IActionResult OnPostEliminar(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            bool eliminado = _pantallasService.EliminarPantalla(id, idUsuario, out string? mensajeError);

            if (!eliminado)
            {
                TempData["Error"] = mensajeError;
                return RedirectToPage("Index");
            }

            TempData["Exito"] = "Pantalla eliminada correctamente.";
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

            var rutasPermitidas = _permisosService.ObtenerRutasPermitidas(idUsuario.Value);
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
