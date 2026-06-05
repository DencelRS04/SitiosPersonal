using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Oferentes.ExperienciaLaboral
{
    public class IndexModel : PageModel
    {
        private readonly ExperienciaLaboralRepository _repository;
        private readonly OferentesRepository _oferentesRepository;
        private readonly BitacoraService _bitacoraService;

        public IndexModel(
            ExperienciaLaboralRepository repository,
            OferentesRepository oferentesRepository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _oferentesRepository = oferentesRepository;
            _bitacoraService = bitacoraService;
        }

        public ExperienciaLaboralListaViewModel Lista { get; set; } = new ExperienciaLaboralListaViewModel();

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

            Lista = new ExperienciaLaboralListaViewModel
            {
                id_oferente    = idOferente,
                NombreOferente = oferente.nombre_completo,
                Registros      = _repository.ListarPorOferente(idOferente)
            };

            _bitacoraService.RegistrarConsulta(idUsuario, $"ExperienciaLaboral (Oferente {idOferente})");

            return Page();
        }

        public IActionResult OnPostEliminar(int idOferente, int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var experiencia = _repository.ObtenerPorId(id);

            if (experiencia == null)
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
                "ExperienciaLaboral",
                new { experiencia.id_experiencia, experiencia.id_oferente, experiencia.empresa, experiencia.puesto }
            );

            TempData["Exito"] = "Registro eliminado correctamente.";
            return RedirectToPage("Index", new { idOferente });
        }
    }
}
