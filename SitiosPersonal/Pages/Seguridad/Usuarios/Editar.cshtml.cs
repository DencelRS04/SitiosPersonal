using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Helpers;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Usuarios
{
    public class EditarModel : PageModel
    {
        private readonly UsuariosService _repository;
        private readonly BitacoraService _bitacoraService;
        private readonly EncryptionHelper _encryptionHelper;

        public EditarModel(
            UsuariosService repository,
            BitacoraService bitacoraService,
            EncryptionHelper encryptionHelper)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
            _encryptionHelper = encryptionHelper;
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

            var usuario = _repository.ObtenerPorId(id);

            if (usuario == null)
            {
                return RedirectToPage("Index");
            }

            string passwordDesencriptado = string.Empty;

            if (!string.IsNullOrWhiteSpace(usuario.password_hash))
            {
                passwordDesencriptado = _encryptionHelper.Desencriptar(usuario.password_hash);
            }

            Usuario = new UsuarioViewModel
            {
                id_usuario = usuario.id_usuario,
                usuario = usuario.usuario,
                nombre_completo = usuario.nombre_completo,
                correo = usuario.correo,
                password = passwordDesencriptado,
                estado = usuario.estado,
                RolesDisponibles = _repository.ListarRoles(),
                RolesSeleccionados = _repository.ObtenerRolesDelUsuario(id)
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
                Usuario.RolesDisponibles = _repository.ListarRoles();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var usuarioAnterior = _repository.ObtenerPorId(id);

            if (usuarioAnterior == null)
            {
                return RedirectToPage("Index");
            }

            var rolesAnteriores = _repository.ObtenerRolesDelUsuario(id);

            string passwordFinal = usuarioAnterior.password_hash;

            if (!string.IsNullOrWhiteSpace(Usuario.password))
            {
                passwordFinal = _encryptionHelper.Encriptar(Usuario.password);
            }

            var usuarioActual = new Usuario
            {
                id_usuario = id,
                usuario = Usuario.usuario,
                nombre_completo = Usuario.nombre_completo,
                correo = Usuario.correo,
                password_hash = passwordFinal,
                estado = Usuario.estado
            };

            _repository.Actualizar(usuarioActual, Usuario.RolesSeleccionados);

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Usuario",
                new
                {
                    usuarioAnterior.id_usuario,
                    usuarioAnterior.usuario,
                    usuarioAnterior.nombre_completo,
                    usuarioAnterior.correo,
                    usuarioAnterior.estado,
                    roles = rolesAnteriores
                },
                new
                {
                    usuarioActual.id_usuario,
                    usuarioActual.usuario,
                    usuarioActual.nombre_completo,
                    usuarioActual.correo,
                    usuarioActual.estado,
                    roles = Usuario.RolesSeleccionados
                });

            TempData["Exito"] = "Usuario actualizado correctamente.";
            return RedirectToPage("Index");
        }
    }
}