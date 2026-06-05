using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Oferentes.PreparacionAcademica
{
    public class CrearModel : PageModel
    {
        private readonly PreparacionAcademicaService _repository;
        private readonly OferentesService _oferentesRepository;
        private readonly BitacoraService _bitacoraService;

        public CrearModel(
            PreparacionAcademicaService repository,
            OferentesService oferentesRepository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _oferentesRepository = oferentesRepository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public PreparacionAcademicaViewModel Preparacion { get; set; } = new PreparacionAcademicaViewModel();

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

            Preparacion.id_oferente = idOferente;
            Preparacion.NombreOferente = oferente.nombre_completo;
            Preparacion.InstitucionesDisponibles = _repository.ListarInstituciones();

            return Page();
        }

        public IActionResult OnPost(int idOferente)
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

            var preparacion = new Entities.Models.PreparacionAcademica
            {
                id_oferente = idOferente,
                id_institucion = Preparacion.id_institucion!.Value,
                titulo = Preparacion.titulo.Trim(),
                fecha_inicio = Preparacion.fecha_inicio!.Value,
                fecha_fin = Preparacion.fecha_fin!.Value
            };

            int idPreparacion = _repository.Crear(preparacion);
            preparacion.id_preparacion = idPreparacion;

            _bitacoraService.RegistrarInsert(
                idUsuario,
                "PreparacionAcademica",
                new { preparacion.id_preparacion, preparacion.id_oferente, preparacion.titulo }
            );

            TempData["Exito"] = "Preparación académica registrada correctamente.";
            return RedirectToPage("Index", new { idOferente });
        }
    }
}
