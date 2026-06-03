using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Roles
{
    public class EditarModel : PageModel
    {
        private readonly RolesRepository _repository;
        private readonly BitacoraService _bitacoraService;

        public EditarModel(RolesRepository repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
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

            var rol = _repository.ObtenerPorId(id);
            if (rol == null)
            {
                return RedirectToPage("Index");
            }

            Rol = new RolViewModel
            {
                id_rol = rol.id_rol,
                nombre = rol.nombre,
                activo = rol.activo,
                PantallasDisponibles = _repository.ListarPantallas(),
                PantallasSeleccionadas = _repository.ObtenerPantallasDelRol(id)
            };

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (!ModelState.IsValid)
            {
                Rol.PantallasDisponibles = _repository.ListarPantallas();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var rolAnterior = _repository.ObtenerPorId(id);
            var pantallasAnteriores = _repository.ObtenerPantallasDelRol(id);

            if (rolAnterior == null)
            {
                return RedirectToPage("Index");
            }

            var rolActual = new Rol
            {
                id_rol = id,
                nombre = Rol.nombre,
                activo = Rol.activo
            };

            _repository.Actualizar(rolActual, Rol.PantallasSeleccionadas);

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Rol",
                new { rolAnterior.id_rol, rolAnterior.nombre, pantallas = pantallasAnteriores },
                new { rolActual.id_rol, rolActual.nombre, pantallas = Rol.PantallasSeleccionadas }
            );

            TempData["Exito"] = "Rol actualizado correctamente.";
            return RedirectToPage("Index");
        }
    }
}
