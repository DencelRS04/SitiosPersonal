using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.InstitucionesEducativas
{
    public class EditarModel : PageModel
    {
        private readonly InstitucionEducativaService _repository;
        private readonly BitacoraService _bitacoraService;

        public EditarModel(InstitucionEducativaService repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public InstitucionEducativaViewModel Institucion { get; set; } = new InstitucionEducativaViewModel();

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var inst = _repository.ObtenerPorId(id);
            if (inst == null) return RedirectToPage("Index");

            Institucion = new InstitucionEducativaViewModel
            {
                id_institucion = inst.id_institucion,
                codigo = inst.codigo,
                nombre = inst.nombre
            };
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (!ModelState.IsValid) return Page();

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var anterior = _repository.ObtenerPorId(id);
            if (anterior == null) return RedirectToPage("Index");

            if (_repository.ExisteCodigo(Institucion.codigo, id))
            {
                ModelState.AddModelError("Institucion.codigo", "Ya existe una institución con ese código.");
                return Page();
            }

            var actual = new InstitucionEducativa { id_institucion = id, codigo = Institucion.codigo, nombre = Institucion.nombre };
            _repository.Actualizar(actual);

            _bitacoraService.RegistrarUpdate(idUsuario, "Institución Educativa",
                new { anterior.id_institucion, anterior.codigo, anterior.nombre },
                new { actual.id_institucion, actual.codigo, actual.nombre });

            TempData["Exito"] = "Institución educativa actualizada correctamente.";
            return RedirectToPage("Index");
        }
    }
}
