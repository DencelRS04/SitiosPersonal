using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Empleados
{
    public class ContratarModel : PageModel
    {
        private readonly EmpleadosService _empleadosService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosService;

        [BindProperty]
        public ContratarEmpleadoViewModel Input { get; set; } = new();

        public List<OferenteDropdownItem> OferentesDisponibles { get; set; } = new();
        public List<PuestoDropdownItem> PuestosDisponibles { get; set; } = new();

        public ContratarModel(EmpleadosService empleadosService, BitacoraService bitacoraService, PermisosService permisosService)
        {
            _empleadosService = empleadosService;
            _bitacoraService = bitacoraService;
            _permisosService = permisosService;
        }

        public IActionResult OnGet()
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            CargarDropdowns();
            return Page();
        }

        public IActionResult OnPost()
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            if (!ModelState.IsValid)
            {
                CargarDropdowns();
                return Page();
            }

            var (exito, error) = _empleadosService.Contratar(Input);

            if (!exito)
            {
                ModelState.AddModelError(string.Empty, error);
                CargarDropdowns();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            _bitacoraService.RegistrarInsert(idUsuario, "Empleado", new
            {
                Input.id_oferente,
                Input.id_puesto
            });

            TempData["Exito"] = "Empleado contratado correctamente.";
            return RedirectToPage("Index");
        }

        private void CargarDropdowns()
        {
            var vm = _empleadosService.ObtenerFormularioContratar();
            OferentesDisponibles = vm.OferentesDisponibles;
            PuestosDisponibles = vm.PuestosDisponibles;
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
