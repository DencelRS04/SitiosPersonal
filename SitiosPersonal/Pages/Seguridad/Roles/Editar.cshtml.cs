using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Exceptions;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Roles
{
    public class EditarModel : PageModel
    {
        private readonly RolesService _rolesService;

        public EditarModel(RolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [BindProperty]
        public RolViewModel Rol { get; set; } = new RolViewModel();

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var rol = _rolesService.ObtenerPorId(id);

            if (rol == null)
            {
                return RedirectToPage("Index");
            }

            Rol = new RolViewModel
            {
                id_rol = rol.id_rol,
                nombre = rol.nombre,
                activo = rol.activo,
                PantallasDisponibles = _rolesService.ListarPantallas(),
                PantallasSeleccionadas = _rolesService.ObtenerPantallasDelRol(id)
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
                Rol.PantallasDisponibles = _rolesService.ListarPantallas();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var rolActual = new Rol
            {
                id_rol = id,
                nombre = Rol.nombre,
                activo = Rol.activo
            };

            try
            {
                _rolesService.ActualizarRol(rolActual, Rol.PantallasSeleccionadas, idUsuario);

                TempData["Exito"] = "Rol actualizado correctamente.";
                return RedirectToPage("Index");
            }
            catch (ValidacionException ex)
            {
                ViewData["ErrorModal"] = ex.Message;
                ModelState.AddModelError(string.Empty, ex.Message);

                Rol.id_rol = id;
                Rol.PantallasDisponibles = _rolesService.ListarPantallas();

                return Page();
            }
        }
    }
}