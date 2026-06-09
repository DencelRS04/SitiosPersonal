using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.AccionesPersonal
{
    public class EditarModel : PageModel
    {
        private readonly AccionesPersonalService _accionesService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosService;

        [BindProperty]
        public AccionPersonalViewModel Input { get; set; } = new();

        public List<EmpleadoDropdownItem> EmpleadosDisponibles { get; set; } = new();

        public EditarModel(AccionesPersonalService accionesService, BitacoraService bitacoraService, PermisosService permisosService)
        {
            _accionesService = accionesService;
            _bitacoraService = bitacoraService;
            _permisosService = permisosService;
        }

        public IActionResult OnGet(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            var vm = _accionesService.ObtenerFormularioEditar(id);
            if (vm == null) return NotFound();

            Input = vm;
            EmpleadosDisponibles = vm.EmpleadosDisponibles;
            return Page();
        }

        public IActionResult OnPost()
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            if (!ModelState.IsValid)
            {
                EmpleadosDisponibles = _accionesService.ObtenerFormularioCrear().EmpleadosDisponibles;
                return Page();
            }

            var (exito, error) = _accionesService.Actualizar(Input);

            if (!exito)
            {
                ModelState.AddModelError("Input.descripcion", error);
                EmpleadosDisponibles = _accionesService.ObtenerFormularioCrear().EmpleadosDisponibles;
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            _bitacoraService.RegistrarUpdate(idUsuario, "AccionPersonal",
                new { id_accion = Input.id_accion },
                new { Input.id_empleado, Input.id_empleado_jefatura, Input.fecha_accion, Input.descripcion });

            TempData["Exito"] = "Acción de personal actualizada correctamente.";
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
