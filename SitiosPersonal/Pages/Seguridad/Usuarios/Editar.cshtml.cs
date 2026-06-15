using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Exceptions;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Usuarios
{
    public class EditarModel : PageModel
    {
        private readonly UsuariosService _usuariosService;

        public EditarModel(UsuariosService usuariosService)
        {
            _usuariosService = usuariosService;
        }

        [BindProperty]
        public UsuarioViewModel Usuario { get; set; } = new UsuarioViewModel();

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var usuario = _usuariosService.ObtenerPorId(id);

            if (usuario == null)
            {
                return RedirectToPage("Index");
            }

            Usuario = new UsuarioViewModel
            {
                id_usuario = usuario.id_usuario,
                usuario = usuario.usuario,
                nombre_completo = usuario.nombre_completo,
                correo = usuario.correo,

                // No se carga la contraseña real.
                // En la vista se muestra ******** como placeholder.
                password = string.Empty,

                estado = usuario.estado,
                RolesDisponibles = _usuariosService.ListarRoles(),
                RolesSeleccionados = _usuariosService.ObtenerRolesDelUsuario(id)
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

            /*
             * En edición la contraseña no es obligatoria.
             * Si viene vacía, el Service conserva la contraseña actual.
             * Si viene con valor, el Service la valida, la encripta y la actualiza.
             */
            if (string.IsNullOrWhiteSpace(Usuario.password))
            {
                ModelState.Remove("Usuario.password");
            }

            if (!ModelState.IsValid)
            {
                Usuario.RolesDisponibles = _usuariosService.ListarRoles();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            try
            {
                _usuariosService.ActualizarUsuario(id, Usuario, idUsuario);

                TempData["Exito"] = "Usuario actualizado correctamente.";
                return RedirectToPage("Index");
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                Usuario.id_usuario = id;
                Usuario.RolesDisponibles = _usuariosService.ListarRoles();

                return Page();
            }
        }
    }
}