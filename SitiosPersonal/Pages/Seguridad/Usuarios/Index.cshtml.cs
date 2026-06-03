using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Seguridad.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly UsuariosRepository _repository;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosRepository _permisosRepository;

        public IndexModel(UsuariosRepository repository, BitacoraService bitacoraService, PermisosRepository permisosRepository)
        { _repository = repository; _bitacoraService = bitacoraService; _permisosRepository = permisosRepository; }

        public UsuariosListaViewModel Lista { get; set; } = new UsuariosListaViewModel();

        public IActionResult OnGet(int pagina = 1)
        {
            var resultado = ValidarAcceso(); if (resultado != null) return resultado;
            int cantidadPorPagina = 10; int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            Lista = new UsuariosListaViewModel { Pagina = pagina, CantidadPorPagina = cantidadPorPagina, TotalRegistros = _repository.Contar(), Usuarios = _repository.ListarPaginado(pagina, cantidadPorPagina) };
            _bitacoraService.RegistrarConsulta(idUsuario, "Usuarios");
            return Page();
        }
        public IActionResult OnPostEliminar(int id)
        {
            var resultado = ValidarAcceso(); if (resultado != null) return resultado;
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario"); var usuario = _repository.ObtenerPorId(id); if (usuario == null) return RedirectToPage("Index");
            if (!_repository.PuedeEliminar(id)) { TempData["Error"] = "No se puede eliminar un registro con datos relacionados."; return RedirectToPage("Index"); }
            _repository.Eliminar(id);
            _bitacoraService.RegistrarDelete(idUsuario, "Usuario", new { usuario.id_usuario, usuario.usuario, usuario.nombre_completo, usuario.correo, usuario.estado });
            TempData["Exito"] = "Usuario eliminado correctamente."; return RedirectToPage("Index");
        }
        public IActionResult OnPostCambiarEstado(int id)
        {
            var resultado = ValidarAcceso(); if (resultado != null) return resultado;
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario"); var usuario = _repository.ObtenerPorId(id); if (usuario == null) return RedirectToPage("Index");
            if (usuario.estado == "BLOQUEADO") { TempData["Error"] = "No se puede activar o inactivar un usuario bloqueado."; return RedirectToPage("Index"); }
            string nuevoEstado = usuario.estado == "ACTIVO" ? "INACTIVO" : "ACTIVO"; _repository.CambiarEstado(id, nuevoEstado);
            _bitacoraService.RegistrarUpdate(idUsuario, "Usuario", new { usuario.id_usuario, usuario.usuario, usuario.estado }, new { usuario.id_usuario, usuario.usuario, estado = nuevoEstado });
            TempData["Exito"] = "Estado actualizado correctamente."; return RedirectToPage("Index");
        }
        private IActionResult? ValidarAcceso()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) { TempData["Mensaje"] = Request.Cookies.ContainsKey("SesionIniciada") ? "La sesión ha expirado. Por favor inicie sesión nuevamente." : "Por favor inicie sesión para utilizar el sistema"; return RedirectToPage("/Login/Index"); }
            var rutasPermitidas = _permisosRepository.ObtenerRutasPermitidas(idUsuario.Value);
            bool tienePermiso = rutasPermitidas.Any(ruta => !string.IsNullOrWhiteSpace(ruta) && Request.Path.Value!.StartsWith(ruta, StringComparison.OrdinalIgnoreCase));
            if (!tienePermiso) { TempData["Error"] = "No tiene permisos para acceder a esta pantalla."; return RedirectToPage("/Home/Index"); }
            return null;
        }
    }
}
