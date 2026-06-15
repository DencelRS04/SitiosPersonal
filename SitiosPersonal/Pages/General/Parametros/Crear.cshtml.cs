using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.Parametros
{
    public class CrearModel : PageModel
    {
        private readonly ParametroService _repository;
        private readonly BitacoraService _bitacoraService;

        public CrearModel(ParametroService repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public ParametroViewModel Parametro { get; set; } = new ParametroViewModel();

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

            var parametro = new Parametro { codigo = Parametro.codigo, valor = Parametro.valor };

            // Validación en la capa de negocio
            var errores = _repository.Validar(parametro);
            if (errores.Count > 0)
            {
                foreach (var error in errores) ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            int id = _repository.Crear(parametro);
            parametro.id_parametro = id;

            _bitacoraService.RegistrarInsert(idUsuario, "Parámetro", new { parametro.id_parametro, parametro.codigo, parametro.valor });
            TempData["Exito"] = "Parámetro creado correctamente.";
            return RedirectToPage("Index");
        }
    }
}
