using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Modulos
{
    public class CrearModel : PageModel
    {
        private readonly PantallasRepository _repository;
        private readonly BitacoraService _bitacoraService;
        public CrearModel(PantallasRepository repository, BitacoraService bitacoraService) { _repository = repository; _bitacoraService = bitacoraService; }
        [BindProperty] public PantallaViewModel Pantalla { get; set; } = new PantallaViewModel();
        public IActionResult OnGet() { if (HttpContext.Session.GetInt32("IdUsuario") == null) { TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema"; return RedirectToPage("/Login/Index"); } Pantalla.RolesDisponibles = _repository.ListarRoles(); return Page(); }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) { Pantalla.RolesDisponibles = _repository.ListarRoles(); return Page(); }
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var pantalla = new Pantalla { nombre = Pantalla.nombre, modulo = "Seguridad", ruta = "#", icono = "fa-window-maximize", orden_menu = 99, visible_menu = false, activo = true };
            int idPantalla = _repository.Crear(pantalla, Pantalla.RolesSeleccionados); pantalla.id_pantalla = idPantalla;
            _bitacoraService.RegistrarInsert(idUsuario, "Pantalla", new { pantalla.id_pantalla, pantalla.nombre, roles = Pantalla.RolesSeleccionados });
            TempData["Exito"] = "Pantalla creada correctamente.";
            return RedirectToPage("Index");
        }
    }
}
