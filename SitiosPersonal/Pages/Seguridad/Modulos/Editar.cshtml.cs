using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Modulos
{
    public class EditarModel : PageModel
    {
        private readonly PantallasService _repository;
        private readonly BitacoraService _bitacoraService;
        public EditarModel(PantallasService repository, BitacoraService bitacoraService) { _repository = repository; _bitacoraService = bitacoraService; }
        [BindProperty] public PantallaViewModel Pantalla { get; set; } = new PantallaViewModel();
        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null) { TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema"; return RedirectToPage("/Login/Index"); }
            var pantalla = _repository.ObtenerPorId(id); if (pantalla == null) return RedirectToPage("Index");
            Pantalla = new PantallaViewModel { id_pantalla = pantalla.id_pantalla, nombre = pantalla.nombre, RolesDisponibles = _repository.ListarRoles(), RolesSeleccionados = _repository.ObtenerRolesDePantalla(id) };
            return Page();
        }
        public IActionResult OnPost(int id)
        {
            if (!ModelState.IsValid) { Pantalla.RolesDisponibles = _repository.ListarRoles(); return Page(); }
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var pantallaAnterior = _repository.ObtenerPorId(id); if (pantallaAnterior == null) return RedirectToPage("Index");
            var rolesAnteriores = _repository.ObtenerRolesDePantalla(id);
            var pantallaActual = new Pantalla { id_pantalla = id, nombre = Pantalla.nombre };
            _repository.Actualizar(pantallaActual, Pantalla.RolesSeleccionados);
            _bitacoraService.RegistrarUpdate(idUsuario, "Pantalla", new { pantallaAnterior.id_pantalla, pantallaAnterior.nombre, roles = rolesAnteriores }, new { pantallaActual.id_pantalla, pantallaActual.nombre, roles = Pantalla.RolesSeleccionados });
            TempData["Exito"] = "Pantalla actualizada correctamente.";
            return RedirectToPage("Index");
        }
    }
}
