using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Oferentes.PreparacionAcademica
{
    public class EditarModel : PageModel
    {
        private readonly PreparacionAcademicaRepository _repository;
        private readonly OferentesRepository _oferentesRepository;
        private readonly BitacoraService _bitacoraService;

        public EditarModel(
            PreparacionAcademicaRepository repository,
            OferentesRepository oferentesRepository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _oferentesRepository = oferentesRepository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public PreparacionAcademicaViewModel Preparacion { get; set; } = new PreparacionAcademicaViewModel();

        public IActionResult OnGet(int idOferente, int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var preparacion = _repository.ObtenerPorId(id);
            if (preparacion == null)
            {
                return RedirectToPage("Index", new { idOferente });
            }

            var oferente = _oferentesRepository.ObtenerPorId(idOferente);

            Preparacion = new PreparacionAcademicaViewModel
            {
                id_preparacion       = preparacion.id_preparacion,
                id_oferente          = idOferente,
                NombreOferente       = oferente?.nombre_completo ?? "",
                id_institucion       = preparacion.id_institucion,
                titulo               = preparacion.titulo,
                fecha_inicio         = preparacion.fecha_inicio,
                fecha_fin            = preparacion.fecha_fin,
                InstitucionesDisponibles = _repository.ListarInstituciones()
            };

            return Page();
        }

        public IActionResult OnPost(int idOferente, int id)
        {
            if (!ModelState.IsValid)
            {
                Preparacion.InstitucionesDisponibles = _repository.ListarInstituciones();
                return Page();
            }

            if (Preparacion.fecha_fin < Preparacion.fecha_inicio)
            {
                ModelState.AddModelError("", "La fecha de fin debe ser mayor o igual a la fecha de inicio.");
                Preparacion.InstitucionesDisponibles = _repository.ListarInstituciones();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var anterior = _repository.ObtenerPorId(id);

            if (anterior == null)
            {
                return RedirectToPage("Index", new { idOferente });
            }

            var actualizado = new Entities.Models.PreparacionAcademica
            {
                id_preparacion = id,
                id_oferente    = idOferente,
                id_institucion = Preparacion.id_institucion!.Value,
                titulo         = Preparacion.titulo.Trim(),
                fecha_inicio   = Preparacion.fecha_inicio!.Value,
                fecha_fin      = Preparacion.fecha_fin!.Value
            };

            _repository.Actualizar(actualizado);

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "PreparacionAcademica",
                new { anterior.id_preparacion, anterior.id_oferente, anterior.titulo, anterior.id_institucion },
                new { actualizado.id_preparacion, actualizado.id_oferente, actualizado.titulo, actualizado.id_institucion }
            );

            TempData["Exito"] = "Preparación académica actualizada correctamente.";
            return RedirectToPage("Index", new { idOferente });
        }
    }
}
