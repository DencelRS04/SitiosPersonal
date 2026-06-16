using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly UsuariosService _usuariosService;
        private readonly PermisosService _permisosService;

        public IndexModel(
            UsuariosService usuariosService,
            PermisosService permisosService)
        {
            _usuariosService = usuariosService;
            _permisosService = permisosService;
        }

        public UsuariosListaViewModel Lista { get; set; } = new UsuariosListaViewModel();

        public UsuariosListaViewModel ViewModel => Lista;

        public IActionResult OnGet(int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            Lista = _usuariosService.ObtenerListado(pagina, 10, idUsuario);

            return Page();
        }

        public IActionResult OnPostEliminar(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            bool eliminado = _usuariosService.EliminarUsuario(id, idUsuario, out string? mensajeError);

            if (!eliminado)
            {
                TempData["ErrorModal"] = mensajeError ?? "No se puede eliminar un registro con datos relacionados.";
                return RedirectToPage("Index");
            }

            TempData["Exito"] = "Usuario eliminado correctamente.";
            return RedirectToPage("Index");
        }

        public IActionResult OnPostCambiarEstado(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            bool actualizado = _usuariosService.CambiarEstadoUsuario(id, idUsuario, out string? mensajeError);

            if (!actualizado)
            {
                TempData["ErrorModal"] = mensajeError ?? "No se pudo actualizar el estado del usuario.";
                return RedirectToPage("Index");
            }

            TempData["Exito"] = "Estado actualizado correctamente.";
            return RedirectToPage("Index");
        }

        private IActionResult? ValidarAcceso()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                TempData["Mensaje"] = Request.Cookies.ContainsKey("SesionIniciada")
                    ? "La sesión ha expirado. Por favor inicie sesión nuevamente."
                    : "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var rutasPermitidas = _permisosService.ObtenerRutasPermitidas(idUsuario.Value);
            bool tienePermiso = rutasPermitidas.Any(ruta =>
                !string.IsNullOrWhiteSpace(ruta)
                && Request.Path.Value!.StartsWith(ruta, StringComparison.OrdinalIgnoreCase));

            if (!tienePermiso)
            {
                TempData["ErrorModal"] = "No tiene permisos para acceder a esta pantalla.";
                return RedirectToPage("/Home/Index");
            }

            return null;
        }
    }
}