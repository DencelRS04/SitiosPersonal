using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Puestos
{
    public class IndexModel : PageModel
    {
        private readonly PuestosService _puestosService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosService;
        private const int CantidadPorPagina = 10;

        public PuestosListaViewModel Lista { get; set; } = new();

        public PuestosListaViewModel ViewModel => Lista;

        public IndexModel(PuestosService puestosService, BitacoraService bitacoraService, PermisosService permisosService)
        {
            _puestosService = puestosService;
            _bitacoraService = bitacoraService;
            _permisosService = permisosService;
        }

        public IActionResult OnGet(int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            Lista = _puestosService.ObtenerPaginado(pagina, CantidadPorPagina);

            _bitacoraService.RegistrarConsulta(idUsuario, "Puesto");

            return Page();
        }

        public IActionResult OnPostEliminar(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var puesto = _puestosService.ObtenerPorId(id);
            if (puesto == null) return RedirectToPage();

            var (exito, error) = _puestosService.Eliminar(id);

            if (!exito)
            {
                TempData["Error"] = error;
            }
            else
            {
                _bitacoraService.RegistrarDelete(idUsuario, "Puesto", new { puesto.id_puesto, puesto.codigo, puesto.nombre });
                TempData["Exito"] = "Puesto eliminado correctamente.";
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
