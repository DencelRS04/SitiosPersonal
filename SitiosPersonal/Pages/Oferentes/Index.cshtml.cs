using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Oferentes
{
    public class IndexModel : PageModel
    {
        private readonly OferentesService _repository;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosRepository;

        public IndexModel(
            OferentesService repository,
            BitacoraService bitacoraService,
            PermisosService permisosRepository)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
            _permisosRepository = permisosRepository;
        }

        public OferentesListaViewModel Lista { get; set; } = new OferentesListaViewModel();

        public OferentesListaViewModel ViewModel => Lista;

        public IActionResult OnGet(int pagina = 1)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int cantidadPorPagina = 10;
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            Lista = new OferentesListaViewModel
            {
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _repository.Contar(),
                Oferentes = _repository.ListarPaginado(pagina, cantidadPorPagina)
            };

            _bitacoraService.RegistrarConsulta(idUsuario, "Oferentes");

            return Page();
        }

        public IActionResult OnPostEliminar(int id)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var oferente = _repository.ObtenerPorId(id);

            if (oferente == null)
            {
                return RedirectToPage("Index");
            }

            if (!_repository.PuedeEliminar(id))
            {
                TempData["Error"] = "No se puede eliminar un registro con datos relacionados.";
                return RedirectToPage("Index");
            }

            _repository.Eliminar(id);

            _bitacoraService.RegistrarDelete(
                idUsuario,
                "Oferente",
                new { oferente.id_oferente, oferente.identificacion, oferente.nombre_completo }
            );

            TempData["Exito"] = "Oferente eliminado correctamente.";
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

            var rutasPermitidas = _permisosRepository.ObtenerRutasPermitidas(idUsuario.Value);
            bool tienePermiso = rutasPermitidas.Any(ruta =>
                !string.IsNullOrWhiteSpace(ruta)
                && Request.Path.Value!.StartsWith(ruta, StringComparison.OrdinalIgnoreCase));

            if (!tienePermiso)
            {
                TempData["Error"] = "No tiene permisos para acceder a esta pantalla.";
                return RedirectToPage("/Home/Index");
            }

            return null;
        }
    }
}
