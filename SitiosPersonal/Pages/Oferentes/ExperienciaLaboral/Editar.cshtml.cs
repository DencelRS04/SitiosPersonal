using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Oferentes.ExperienciaLaboral
{
    public class EditarModel : PageModel
    {
        private readonly ExperienciaLaboralService _repository;
        private readonly OferentesService _oferentesRepository;
        private readonly BitacoraService _bitacoraService;

        public EditarModel(
            ExperienciaLaboralService repository,
            OferentesService oferentesRepository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _oferentesRepository = oferentesRepository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public ExperienciaLaboralViewModel Experiencia { get; set; } = new ExperienciaLaboralViewModel();

        public IActionResult OnGet(int idOferente, int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var experiencia = _repository.ObtenerPorId(id);
            if (experiencia == null)
            {
                return RedirectToPage("Index", new { idOferente });
            }

            var oferente = _oferentesRepository.ObtenerPorId(idOferente);

            Experiencia = new ExperienciaLaboralViewModel
            {
                id_experiencia = experiencia.id_experiencia,
                id_oferente = idOferente,
                NombreOferente = oferente?.nombre_completo ?? "",
                empresa = experiencia.empresa,
                puesto = experiencia.puesto,
                fecha_inicio = experiencia.fecha_inicio,
                fecha_fin = experiencia.fecha_fin
            };

            return Page();
        }

        public IActionResult OnPost(int idOferente, int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Experiencia.fecha_fin < Experiencia.fecha_inicio)
            {
                ModelState.AddModelError("", "La fecha de fin debe ser mayor o igual a la fecha de inicio.");
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var anterior = _repository.ObtenerPorId(id);

            if (anterior == null)
            {
                return RedirectToPage("Index", new { idOferente });
            }

            var actualizado = new Entities.Models.ExperienciaLaboral
            {
                id_experiencia = id,
                id_oferente = idOferente,
                empresa = Experiencia.empresa.Trim(),
                puesto = Experiencia.puesto.Trim(),
                fecha_inicio = Experiencia.fecha_inicio!.Value,
                fecha_fin = Experiencia.fecha_fin!.Value
            };

            _repository.Actualizar(actualizado);

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "ExperienciaLaboral",
                new { anterior.id_experiencia, anterior.id_oferente, anterior.empresa, anterior.puesto },
                new { actualizado.id_experiencia, actualizado.id_oferente, actualizado.empresa, actualizado.puesto }
            );

            TempData["Exito"] = "Experiencia laboral actualizada correctamente.";
            return RedirectToPage("Index", new { idOferente });
        }
    }
}
