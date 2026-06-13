using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Puestos.RequisitoPuestos
{
    public class IndexModel : PageModel
    {
        private readonly RequisitoPuestosService _requisitoService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosService;
        private const int CantidadPorPagina = 10;

        public RequisitoPuestoListaViewModel Lista { get; set; } = new();

        public RequisitoPuestoListaViewModel ViewModel => Lista;

        public IndexModel(RequisitoPuestosService requisitoService, BitacoraService bitacoraService, PermisosService permisosService)
        {
            _requisitoService = requisitoService;
            _bitacoraService = bitacoraService;
            _permisosService = permisosService;
        }

        public IActionResult OnGet(int id, int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            var vm = _requisitoService.ObtenerPaginado(id, pagina, CantidadPorPagina);
            if (vm == null) return NotFound();

            Lista = vm;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            _bitacoraService.RegistrarConsulta(idUsuario, "RequisitoPuesto");

            return Page();
        }

        public IActionResult OnPostEliminar(int id, int idRequisito)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var (exito, error) = _requisitoService.Eliminar(idRequisito);

            if (!exito)
            {
                TempData["Error"] = error;
            }
            else
            {
                _bitacoraService.RegistrarDelete(idUsuario, "RequisitoPuesto", new { id_requisito = idRequisito });
                TempData["Exito"] = "Requisito eliminado correctamente.";
            }

            return RedirectToPage(new { id = id });
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
