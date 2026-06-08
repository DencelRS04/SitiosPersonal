using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Areas
{
    public class EditarModel : PageModel
    {
        private readonly AreasService _areasService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosService;

        [BindProperty]
        public AreaViewModel Input { get; set; } = new();

        public List<EmpleadoDropdownItem> EmpleadosDisponibles { get; set; } = new();

        public EditarModel(AreasService areasService, BitacoraService bitacoraService, PermisosService permisosService)
        {
            _areasService = areasService;
            _bitacoraService = bitacoraService;
            _permisosService = permisosService;
        }

        public IActionResult OnGet(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            var vm = _areasService.ObtenerFormularioEditar(id);
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
                EmpleadosDisponibles = _areasService.ObtenerFormularioCrear().EmpleadosDisponibles;
                return Page();
            }

            var (exito, error) = _areasService.Actualizar(Input);

            if (!exito)
            {
                ModelState.AddModelError("Input.codigo", error);
                EmpleadosDisponibles = _areasService.ObtenerFormularioCrear().EmpleadosDisponibles;
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            _bitacoraService.RegistrarUpdate(idUsuario, "Area",
                new { id_area = Input.id_area },
                new { Input.codigo, Input.nombre, Input.id_empleado_jefatura });

            TempData["Exito"] = "Área actualizada correctamente.";
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
