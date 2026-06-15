using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.Bitacora
{
    public class IndexModel : PageModel
    {
        private readonly BitacoraConsultaService _bitacoraConsultaService;
        private readonly PermisosService _permisosService;

        public IndexModel(
            BitacoraConsultaService bitacoraConsultaService,
            PermisosService permisosService)
        {
            _bitacoraConsultaService = bitacoraConsultaService;
            _permisosService = permisosService;
        }

        public BitacoraFiltroViewModel Filtro { get; set; } = new BitacoraFiltroViewModel();

        public IActionResult OnGet(
            string? usuarioFiltro,
            string? descripcionFiltro,
            string orden = "fecha_desc",
            int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            Filtro = _bitacoraConsultaService.ObtenerBitacoras(
                usuarioFiltro,
                descripcionFiltro,
                orden,
                pagina,
                100,
                idUsuario);

            return Page();
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
