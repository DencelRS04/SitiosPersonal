using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Exceptions;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Modulos
{
    public class EditarModel : PageModel
    {
        private readonly PantallasService _pantallasService;

        public EditarModel(PantallasService pantallasService)
        {
            _pantallasService = pantallasService;
        }

        [BindProperty]
        public PantallaViewModel Pantalla { get; set; } = new PantallaViewModel();

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var pantalla = _pantallasService.ObtenerPorId(id);

            if (pantalla == null)
            {
                return RedirectToPage("Index");
            }

            Pantalla = new PantallaViewModel
            {
                id_pantalla = pantalla.id_pantalla,
                nombre = pantalla.nombre,
                RolesDisponibles = _pantallasService.ListarRoles(),
                RolesSeleccionados = _pantallasService.ObtenerRolesDePantalla(id)
            };

            return Page();
        }

        public IActionResult OnPost(int id)
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

            var pantallaActual = new Pantalla
            {
                id_pantalla = id,
                nombre = Pantalla.nombre
            };

            try
            {
                _pantallasService.ActualizarPantalla(pantallaActual, Pantalla.RolesSeleccionados, idUsuario);

                TempData["Exito"] = "Pantalla actualizada correctamente.";
                return RedirectToPage("Index");
            }
            catch (ValidacionException ex)
            {
                ViewData["ErrorModal"] = ex.Message;
                ModelState.AddModelError(string.Empty, ex.Message);

                Pantalla.id_pantalla = id;
                Pantalla.RolesDisponibles = _pantallasService.ListarRoles();

                return Page();
            }
        }
    }
}