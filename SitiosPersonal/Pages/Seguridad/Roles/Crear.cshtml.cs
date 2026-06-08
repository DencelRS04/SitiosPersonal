using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Roles
{
    public class CrearModel : PageModel
    {
        private readonly RolesService _repository;
        private readonly BitacoraService _bitacoraService;

        public CrearModel(RolesService repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
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

            Rol.PantallasDisponibles = _repository.ListarPantallas();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Rol.PantallasDisponibles = _repository.ListarPantallas();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var rol = new Rol
            {
                nombre = Rol.nombre,
                activo = true
            };

            int idRol = _repository.Crear(rol, Rol.PantallasSeleccionadas);
            rol.id_rol = idRol;

            _bitacoraService.RegistrarInsert(
                idUsuario,
                "Rol",
                new { rol.id_rol, rol.nombre, pantallas = Rol.PantallasSeleccionadas }
            );

            TempData["Exito"] = "Rol creado correctamente.";
            return RedirectToPage("Index");
        }
    }
}
