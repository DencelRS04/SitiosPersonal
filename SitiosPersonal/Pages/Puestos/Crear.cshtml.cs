using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Puestos
{
    public class CrearModel : PageModel
    {
        private readonly PuestosService _puestosService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosService;

        [BindProperty]
        public PuestoViewModel Input { get; set; } = new();

        public List<Puesto> PuestosDisponibles { get; set; } = new();

        public CrearModel(PuestosService puestosService, BitacoraService bitacoraService, PermisosService permisosService)
        {
            _puestosService = puestosService;
            _bitacoraService = bitacoraService;
            _permisosService = permisosService;
        }

        public IActionResult OnGet()
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            PuestosDisponibles = _puestosService.ListarTodos();
            return Page();
        }

        public IActionResult OnPost()
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            if (!ModelState.IsValid)
            {
                PuestosDisponibles = _puestosService.ListarTodos();
                return Page();
            }

            var (exito, error) = _puestosService.Crear(Input);

            if (!exito)
            {
                ModelState.AddModelError("Input.codigo", error);
                PuestosDisponibles = _puestosService.ListarTodos();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            _bitacoraService.RegistrarInsert(idUsuario, "Puesto", new
            {
                Input.codigo,
                Input.nombre,
                Input.monto_salario,
                Input.id_puesto_jefatura
            });

            TempData["Exito"] = "Puesto creado correctamente.";
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
