using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Exceptions;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Roles
{
    public class CrearModel : PageModel
    {
        private readonly RolesService _rolesService;

        public CrearModel(RolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [BindProperty]
        public RolViewModel Rol { get; set; } = new RolViewModel();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            Rol.PantallasDisponibles = _rolesService.ListarPantallas();
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
                Rol.PantallasDisponibles = _rolesService.ListarPantallas();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var rol = new Rol
            {
                nombre = Rol.nombre,
                activo = true
            };

            try
            {
                _rolesService.CrearRol(rol, Rol.PantallasSeleccionadas, idUsuario);

                TempData["Exito"] = "Rol creado correctamente.";
                return RedirectToPage("Index");
            }
            catch (ValidacionException ex)
            {
                ViewData["ErrorModal"] = ex.Message;
                ModelState.AddModelError(string.Empty, ex.Message);

                Rol.PantallasDisponibles = _rolesService.ListarPantallas();
                return Page();
            }
        }
    }
}