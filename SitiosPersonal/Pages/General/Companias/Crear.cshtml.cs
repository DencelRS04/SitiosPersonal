using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.Companias
{
    public class CrearModel : PageModel
    {
        private readonly CompaniaService _repository;
        private readonly BitacoraService _bitacoraService;

        public CrearModel(CompaniaService repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public CompaniaViewModel Compania { get; set; } = new CompaniaViewModel();

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

            var compania = new Compania { codigo = Compania.codigo, nombre = Compania.nombre };

            // Validación en la capa de negocio
            var errores = _repository.Validar(compania);
            if (errores.Count > 0)
            {
                foreach (var error in errores) ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            int id = _repository.Crear(compania);
            compania.id_compania = id;

            _bitacoraService.RegistrarInsert(idUsuario, "Compañía", new { compania.id_compania, compania.codigo, compania.nombre });
            TempData["Exito"] = "Compañía creada correctamente.";
            return RedirectToPage("Index");
        }
    }
}
