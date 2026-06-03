using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.Bitacora
{
    public class IndexModel : PageModel
    {
        private readonly BitacoraRepository _repository; private readonly BitacoraService _bitacoraService; private readonly PermisosRepository _permisosRepository;
        public IndexModel(BitacoraRepository repository, BitacoraService bitacoraService, PermisosRepository permisosRepository) { _repository = repository; _bitacoraService = bitacoraService; _permisosRepository = permisosRepository; }
        public BitacoraFiltroViewModel Filtro { get; set; } = new BitacoraFiltroViewModel();
        public IActionResult OnGet(string? usuarioFiltro, string? descripcionFiltro, string orden = "fecha_desc", int pagina = 1)
        {
            var resultado = ValidarAcceso(); if (resultado != null) return resultado;
            int cantidadPorPagina = 100; int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            Filtro = new BitacoraFiltroViewModel { UsuarioFiltro = usuarioFiltro, DescripcionFiltro = descripcionFiltro, Orden = orden, Pagina = pagina, CantidadPorPagina = cantidadPorPagina, TotalRegistros = _repository.Contar(usuarioFiltro, descripcionFiltro), Bitacoras = _repository.Listar(usuarioFiltro, descripcionFiltro, orden, pagina, cantidadPorPagina) };
            _bitacoraService.RegistrarConsulta(idUsuario, "Bitácora");
            return Page();
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
