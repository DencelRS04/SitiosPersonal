using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Concursos
{
    public class EditarModel : PageModel
    {
        private readonly ConcursosRepository _repository;
        private readonly BitacoraService _bitacoraService;

        public EditarModel(ConcursosRepository repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public ConcursoViewModel Concurso { get; set; } = new ConcursoViewModel();

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var concurso = _repository.ObtenerPorId(id);
            if (concurso == null)
            {
                return RedirectToPage("Index");
            }

            Concurso = new ConcursoViewModel
            {
                id_concurso  = concurso.id_concurso,
                codigo       = concurso.codigo,
                nombre       = concurso.nombre,
                fecha_inicio = concurso.fecha_inicio,
                fecha_fin    = concurso.fecha_fin,
                estado       = concurso.estado
            };

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Concurso.fecha_fin < Concurso.fecha_inicio)
            {
                ModelState.AddModelError("", "La fecha de fin no puede ser anterior a la fecha de inicio.");
                return Page();
            }

            if (_repository.ExisteCodigo(Concurso.codigo, id))
            {
                ModelState.AddModelError("", "El código del concurso ya está registrado en el sistema.");
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var concursoAnterior = _repository.ObtenerPorId(id);

            if (concursoAnterior == null)
            {
                return RedirectToPage("Index");
            }

            var concursoActual = new Concurso
            {
                id_concurso  = id,
                codigo       = Concurso.codigo.Trim(),
                nombre       = Concurso.nombre.Trim(),
                fecha_inicio = Concurso.fecha_inicio!.Value,
                fecha_fin    = Concurso.fecha_fin!.Value,
                estado       = Concurso.estado
            };

            _repository.Actualizar(concursoActual);

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Concurso",
                new { concursoAnterior.id_concurso, concursoAnterior.codigo, concursoAnterior.nombre, concursoAnterior.estado },
                new { concursoActual.id_concurso, concursoActual.codigo, concursoActual.nombre, concursoActual.estado }
            );

            TempData["Exito"] = "El concurso ha sido actualizado correctamente.";
            return RedirectToPage("Index");
        }
    }
}
