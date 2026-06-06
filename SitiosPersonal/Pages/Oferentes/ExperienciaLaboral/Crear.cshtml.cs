using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Oferentes.ExperienciaLaboral
{
    public class CrearModel : PageModel
    {
        private readonly ExperienciaLaboralService _repository;
        private readonly OferentesService _oferentesRepository;
        private readonly BitacoraService _bitacoraService;

        public CrearModel(
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

            Experiencia.id_oferente = idOferente;
            Experiencia.NombreOferente = oferente.nombre_completo;

            return Page();
        }

        public IActionResult OnPost(int idOferente)
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

            var experiencia = new Entities.Models.ExperienciaLaboral
            {
                id_oferente = idOferente,
                empresa = Experiencia.empresa.Trim(),
                puesto = Experiencia.puesto.Trim(),
                fecha_inicio = Experiencia.fecha_inicio!.Value,
                fecha_fin = Experiencia.fecha_fin!.Value
            };

            int idExperiencia = _repository.Crear(experiencia);
            experiencia.id_experiencia = idExperiencia;

            _bitacoraService.RegistrarInsert(
                idUsuario,
                "ExperienciaLaboral",
                new { experiencia.id_experiencia, experiencia.id_oferente, experiencia.empresa, experiencia.puesto }
            );

            TempData["Exito"] = "Experiencia laboral registrada correctamente.";
            return RedirectToPage("Index", new { idOferente });
        }
    }
}
