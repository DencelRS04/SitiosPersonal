using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.InstitucionesEducativas
{
    public class CrearModel : PageModel
    {
        private readonly InstitucionEducativaService _repository;
        private readonly BitacoraService _bitacoraService;

        public CrearModel(InstitucionEducativaService repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public InstitucionEducativaViewModel Institucion { get; set; } = new InstitucionEducativaViewModel();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (_repository.ExisteCodigo(Institucion.codigo))
            {
                ModelState.AddModelError("Institucion.codigo", "Ya existe una institución con ese código.");
                return Page();
            }

            var institucion = new InstitucionEducativa { codigo = Institucion.codigo, nombre = Institucion.nombre };
            int id = _repository.Crear(institucion);
            institucion.id_institucion = id;

            _bitacoraService.RegistrarInsert(idUsuario, "Institución Educativa", new { institucion.id_institucion, institucion.codigo, institucion.nombre });
            TempData["Exito"] = "Institución educativa creada correctamente.";
            return RedirectToPage("Index");
        }
    }
}
