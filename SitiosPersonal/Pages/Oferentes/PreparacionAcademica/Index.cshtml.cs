using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Oferentes.PreparacionAcademica
{
    public class IndexModel : PageModel
    {
        private readonly PreparacionAcademicaRepository _repository;
        private readonly OferentesRepository _oferentesRepository;
        private readonly BitacoraService _bitacoraService;

        public IndexModel(
            PreparacionAcademicaRepository repository,
            OferentesRepository oferentesRepository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _oferentesRepository = oferentesRepository;
            _bitacoraService = bitacoraService;
        }

        public PreparacionAcademicaListaViewModel Lista { get; set; } = new PreparacionAcademicaListaViewModel();

        public IActionResult OnGet(int idOferente)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var oferente = _oferentesRepository.ObtenerPorId(idOferente);
            if (oferente == null)
            {
                return RedirectToPage("/Oferentes/Index");
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            Lista = new PreparacionAcademicaListaViewModel
            {
                id_oferente    = idOferente,
                NombreOferente = oferente.nombre_completo,
                Registros      = _repository.ListarPorOferente(idOferente)
            };

            _bitacoraService.RegistrarConsulta(idUsuario, $"PreparacionAcademica (Oferente {idOferente})");

            return Page();
        }

        public IActionResult OnPostEliminar(int idOferente, int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var preparacion = _repository.ObtenerPorId(id);

            if (preparacion == null)
            {
                return RedirectToPage("Index", new { idOferente });
            }

            if (!_repository.PuedeEliminar(id))
            {
                TempData["Error"] = "No se puede eliminar un registro con datos relacionados.";
                return RedirectToPage("Index", new { idOferente });
            }

            _repository.Eliminar(id);

            _bitacoraService.RegistrarDelete(
                idUsuario,
                "PreparacionAcademica",
                new { preparacion.id_preparacion, preparacion.id_oferente, preparacion.titulo }
            );

            TempData["Exito"] = "Registro eliminado correctamente.";
            return RedirectToPage("Index", new { idOferente });
        }
    }
}
