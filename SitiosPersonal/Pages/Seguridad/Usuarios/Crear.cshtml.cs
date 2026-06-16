using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Exceptions;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Usuarios
{
    public class CrearModel : PageModel
    {
        private readonly UsuariosService _usuariosService;

        public CrearModel(UsuariosService usuariosService)
        {
            _usuariosService = usuariosService;
        }

        [BindProperty]
        public UsuarioViewModel Usuario { get; set; } = new UsuarioViewModel();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            Usuario = new UsuarioViewModel
            {
                estado = "ACTIVO",
                password = string.Empty,
                RolesDisponibles = _usuariosService.ListarRoles(),
                RolesSeleccionados = new List<int>()
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            if (string.IsNullOrWhiteSpace(Usuario.password))
            {
                ModelState.AddModelError("Usuario.password", "La contraseña es obligatoria");
            }

            if (!ModelState.IsValid)
            {
                Usuario.RolesDisponibles = _usuariosService.ListarRoles();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            try
            {
                _usuariosService.CrearUsuario(Usuario, idUsuario);

                TempData["Exito"] = "Usuario creado correctamente.";
                return RedirectToPage("Index");
            }
            catch (ValidacionException ex)
            {
                ViewData["ErrorModal"] = ex.Message;
                ModelState.AddModelError(string.Empty, ex.Message);

                Usuario.RolesDisponibles = _usuariosService.ListarRoles();
                return Page();
            }
        }
    }
}