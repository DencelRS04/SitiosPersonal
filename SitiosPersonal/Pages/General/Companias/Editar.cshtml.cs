using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.Companias
{
    public class EditarModel : PageModel
    {
        private readonly CompaniaService _repository;
        private readonly BitacoraService _bitacoraService;

        public EditarModel(CompaniaService repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public CompaniaViewModel Compania { get; set; } = new CompaniaViewModel();

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var compania = _repository.ObtenerPorId(id);
            if (compania == null) return RedirectToPage("Index");

            Compania = new CompaniaViewModel
            {
                id_compania = compania.id_compania,
                codigo = compania.codigo,
                nombre = compania.nombre
            };
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (!ModelState.IsValid) return Page();

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var anterior = _repository.ObtenerPorId(id);
            if (anterior == null) return RedirectToPage("Index");

            var actual = new Compania { id_compania = id, codigo = Compania.codigo, nombre = Compania.nombre };

            // Validación en la capa de negocio
            var errores = _repository.Validar(actual, id);
            if (errores.Count > 0)
            {
                foreach (var error in errores) ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            _repository.Actualizar(actual);

            _bitacoraService.RegistrarUpdate(idUsuario, "Compañía",
                new { anterior.id_compania, anterior.codigo, anterior.nombre },
                new { actual.id_compania, actual.codigo, actual.nombre });

            TempData["Exito"] = "Compañía actualizada correctamente.";
            return RedirectToPage("Index");
        }
    }
}
