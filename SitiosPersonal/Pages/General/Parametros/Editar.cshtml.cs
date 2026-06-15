using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.Parametros
{
    public class EditarModel : PageModel
    {
        private readonly ParametroService _repository;
        private readonly BitacoraService _bitacoraService;

        public EditarModel(ParametroService repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public ParametroViewModel Parametro { get; set; } = new ParametroViewModel();

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var parametro = _repository.ObtenerPorId(id);
            if (parametro == null) return RedirectToPage("Index");

            Parametro = new ParametroViewModel
            {
                id_parametro = parametro.id_parametro,
                codigo = parametro.codigo,
                valor = parametro.valor
            };

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (!ModelState.IsValid) return Page();

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var anterior = _repository.ObtenerPorId(id);
            if (anterior == null) return RedirectToPage("Index");

            var actual = new Parametro { id_parametro = id, codigo = Parametro.codigo, valor = Parametro.valor };

            // Validación en la capa de negocio
            var errores = _repository.Validar(actual, id);
            if (errores.Count > 0)
            {
                foreach (var error in errores) ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            _repository.Actualizar(actual);

            _bitacoraService.RegistrarUpdate(idUsuario, "Parámetro",
                new { anterior.id_parametro, anterior.codigo, anterior.valor },
                new { actual.id_parametro, actual.codigo, actual.valor });

            TempData["Exito"] = "Parámetro actualizado correctamente.";
            return RedirectToPage("Index");
        }
    }
}
