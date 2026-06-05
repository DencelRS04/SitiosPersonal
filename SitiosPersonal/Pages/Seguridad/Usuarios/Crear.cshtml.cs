using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;
using SitiosPersonal.Services.Helpers;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Usuarios
{
    public class CrearModel : PageModel
    {
        private readonly UsuariosService _repository; private readonly BitacoraService _bitacoraService; private readonly EncryptionHelper _encryptionHelper;
        public CrearModel(UsuariosService repository, BitacoraService bitacoraService, EncryptionHelper encryptionHelper) { _repository = repository; _bitacoraService = bitacoraService; _encryptionHelper = encryptionHelper; }
        [BindProperty] public UsuarioViewModel Usuario { get; set; } = new UsuarioViewModel { estado = "ACTIVO" };
        public IActionResult OnGet() { if (HttpContext.Session.GetInt32("IdUsuario") == null) { TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema"; return RedirectToPage("/Login/Index"); } Usuario.RolesDisponibles = _repository.ListarRoles(); Usuario.estado = "ACTIVO"; return Page(); }
        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Usuario.password)) ModelState.AddModelError("Usuario.password", "La contraseña es obligatoria");
            if (!ModelState.IsValid) { Usuario.RolesDisponibles = _repository.ListarRoles(); return Page(); }
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var usuario = new Usuario { usuario = Usuario.usuario, nombre_completo = Usuario.nombre_completo, correo = Usuario.correo, password_hash = _encryptionHelper.Encriptar(Usuario.password!), estado = Usuario.estado };
            int idNuevoUsuario = _repository.Crear(usuario, Usuario.RolesSeleccionados); usuario.id_usuario = idNuevoUsuario;
            _bitacoraService.RegistrarInsert(idUsuario, "Usuario", new { usuario.id_usuario, usuario.usuario, usuario.nombre_completo, usuario.correo, usuario.estado, roles = Usuario.RolesSeleccionados });
            TempData["Exito"] = "Usuario creado correctamente."; return RedirectToPage("Index");
        }
    }
}
