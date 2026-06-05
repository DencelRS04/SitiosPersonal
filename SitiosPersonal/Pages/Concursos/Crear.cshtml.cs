using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Concursos
{
    public class CrearModel : PageModel
    {
        private readonly ConcursosService _repository;
        private readonly BitacoraService _bitacoraService;

        public CrearModel(ConcursosService repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public ConcursoViewModel Concurso { get; set; } = new ConcursoViewModel();

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Concurso.fecha_fin < Concurso.fecha_inicio)
            {
                ModelState.AddModelError("", "La fecha de fin no puede ser anterior a la fecha de inicio.");
                return Page();
            }

            if (_repository.ExisteCodigo(Concurso.codigo))
            {
                ModelState.AddModelError("", "El código del concurso ya está registrado en el sistema.");
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var concurso = new Concurso
            {
                codigo = Concurso.codigo.Trim(),
                nombre = Concurso.nombre.Trim(),
                fecha_inicio = Concurso.fecha_inicio!.Value,
                fecha_fin = Concurso.fecha_fin!.Value,
                estado = Concurso.estado
            };

            int idConcurso = _repository.Crear(concurso);
            concurso.id_concurso = idConcurso;

            _bitacoraService.RegistrarInsert(
                idUsuario,
                "Concurso",
                new { concurso.id_concurso, concurso.codigo, concurso.nombre, concurso.estado }
            );

            TempData["Exito"] = "El concurso ha sido registrado correctamente.";
            return RedirectToPage("Index");
        }
    }
}
