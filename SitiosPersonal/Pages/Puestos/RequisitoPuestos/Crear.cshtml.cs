using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Puestos.RequisitoPuestos
{
    public class CrearModel : PageModel
    {
        private readonly RequisitoPuestosService _requisitoService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosService;

        [BindProperty]
        public RequisitoPuestoViewModel Input { get; set; } = new();

        public string NombrePuesto { get; set; } = "";

        public CrearModel(RequisitoPuestosService requisitoService, BitacoraService bitacoraService, PermisosService permisosService)
        {
            _requisitoService = requisitoService;
            _bitacoraService = bitacoraService;
            _permisosService = permisosService;
        }

        public IActionResult OnGet(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            var vm = _requisitoService.ObtenerFormularioCrear(id);
            if (vm == null) return NotFound();

            Input = vm;
            NombrePuesto = vm.NombrePuesto ?? "";
            return Page();
        }

        public IActionResult OnPost()
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            NombrePuesto = Input.NombrePuesto ?? "";

            if (!ModelState.IsValid) return Page();

            _requisitoService.Crear(Input);

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            _bitacoraService.RegistrarInsert(idUsuario, "RequisitoPuesto", new
            {
                Input.id_puesto,
                Input.nombre
            });

            TempData["Exito"] = "Requisito creado correctamente.";
            return RedirectToPage("Index", new { id = Input.id_puesto });
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
