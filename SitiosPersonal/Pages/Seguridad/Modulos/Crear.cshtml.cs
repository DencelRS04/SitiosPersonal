using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Exceptions;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Modulos
{
    public class CrearModel : PageModel
    {
        private readonly PantallasService _pantallasService;

        public CrearModel(PantallasService pantallasService)
        {
            _pantallasService = pantallasService;
        }

        [BindProperty]
        public PantallaViewModel Pantalla { get; set; } = new PantallaViewModel();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            Pantalla.RolesDisponibles = _pantallasService.ListarRoles();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            if (!ModelState.IsValid)
            {
                Pantalla.RolesDisponibles = _pantallasService.ListarRoles();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var pantalla = new Pantalla
            {
                nombre = Pantalla.nombre,
                modulo = "Seguridad",
                ruta = "#",
                icono = "fa-window-maximize",
                orden_menu = 99,
                visible_menu = false,
                activo = true
            };

            try
            {
                _pantallasService.CrearPantalla(pantalla, Pantalla.RolesSeleccionados, idUsuario);

                TempData["Exito"] = "Pantalla creada correctamente.";
                return RedirectToPage("Index");
            }
            catch (ValidacionException ex)
            {
                ViewData["ErrorModal"] = ex.Message;
                ModelState.AddModelError(string.Empty, ex.Message);

                Pantalla.RolesDisponibles = _pantallasService.ListarRoles();
                return Page();
            }
        }
    }
}