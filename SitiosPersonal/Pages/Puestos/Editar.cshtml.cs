using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Puestos
{
    public class EditarModel : PageModel
    {
        private readonly PuestosService _puestosService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosService;

        [BindProperty]
        public PuestoViewModel Input { get; set; } = new();

        public List<Puesto> PuestosDisponibles { get; set; } = new();

        public EditarModel(PuestosService puestosService, BitacoraService bitacoraService, PermisosService permisosService)
        {
            _puestosService = puestosService;
            _bitacoraService = bitacoraService;
            _permisosService = permisosService;
        }

        public IActionResult OnGet(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            var vm = _puestosService.ObtenerFormularioEditar(id);
            if (vm == null) return NotFound();

            Input = vm;
            PuestosDisponibles = vm.PuestosDisponibles;
            return Page();
        }

        public IActionResult OnPost()
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            if (!ModelState.IsValid)
            {
                PuestosDisponibles = _puestosService.ListarTodos()
                    .Where(p => p.id_puesto != Input.id_puesto).ToList();
                return Page();
            }

            var anterior = _puestosService.ObtenerPorId(Input.id_puesto);
            var (exito, error) = _puestosService.Actualizar(Input);

            if (!exito)
            {
                ModelState.AddModelError("Input.codigo", error);
                PuestosDisponibles = _puestosService.ListarTodos()
                    .Where(p => p.id_puesto != Input.id_puesto).ToList();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            _bitacoraService.RegistrarUpdate(idUsuario, "Puesto",
                new { anterior?.codigo, anterior?.nombre, anterior?.monto_salario },
                new { Input.codigo, Input.nombre, Input.monto_salario });

            TempData["Exito"] = "Puesto actualizado correctamente.";
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
