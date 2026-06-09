using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;
using SitiosPersonal.Services.Helpers;

namespace SitiosPersonal.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly LoginService _repository;
        private readonly EncryptionHelper _encryptionHelper;
        private readonly ParametroService _parametroService;

        public IndexModel(
            LoginService repository,
            EncryptionHelper encryptionHelper,
            ParametroService parametroService)
        {
            _repository = repository;
            _encryptionHelper = encryptionHelper;
            _parametroService = parametroService;
        }

        [BindProperty]
        public LoginViewModel Login { get; set; } = new LoginViewModel();

        public string? Error { get; set; }

        public string? Mensaje { get; set; }

        public void OnGet()
        {
            if (TempData["Mensaje"] != null)
            {
                Mensaje = TempData["Mensaje"]?.ToString();
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Error = "Usuario y/o contraseña incorrectos.";
                return Page();
            }

            var usuario = _repository.ObtenerUsuario(Login.Usuario);

            if (usuario == null)
            {
                Error = "Usuario y/o contraseña incorrectos.";
                return Page();
            }

            if (usuario.estado == "BLOQUEADO")
            {
                Error = "El usuario se encuentra bloqueado.";
                return Page();
            }

            if (usuario.estado == "INACTIVO")
            {
                Error = "El usuario se encuentra inactivo.";
                return Page();
            }

            bool passwordCorrecto = _encryptionHelper.Verificar(
                Login.Password,
                usuario.password_hash
            );

            if (!passwordCorrecto)
            {
                _repository.AumentarIntentos(usuario.id_usuario);

                int intentosActuales = usuario.intentos_login + 1;

                var paramIntentos = _parametroService.ObtenerPorCodigo("MAX_INTENTOS_LOGIN");
                int maxIntentos = (paramIntentos != null && int.TryParse(paramIntentos.valor, out int v)) ? v : 3;

                if (intentosActuales >= maxIntentos)
                {
                    _repository.BloquearUsuario(usuario.id_usuario);
                    Error = $"El usuario se bloqueó por fallar {maxIntentos} intentos.";
                }
                else
                {
                    Error = "Usuario y/o contraseña incorrectos.";
                }

                return Page();
            }

            _repository.ReiniciarIntentos(usuario.id_usuario);

            HttpContext.Session.SetInt32("IdUsuario", usuario.id_usuario);
            HttpContext.Session.SetString("Usuario", usuario.usuario);
            HttpContext.Session.SetString("NombreCompleto", usuario.nombre_completo);

            Response.Cookies.Append(
                "SesionIniciada",
                "1",
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    Expires = DateTimeOffset.Now.AddHours(8)
                }
            );

            return RedirectToPage("/Home/Index");
        }
    }
}
