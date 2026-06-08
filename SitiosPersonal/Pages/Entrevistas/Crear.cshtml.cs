using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Entrevistas
{
    public class CrearModel : PageModel
    {
        private readonly EntrevistasService _repository;
        private readonly BitacoraService _bitacoraService;

        public CrearModel(
            EntrevistasService repository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
            Entrevista = new EntrevistaViewModel();
        }

        [BindProperty]
        public EntrevistaViewModel Entrevista { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            CargarDropdowns();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                CargarDropdowns();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var entrevista = new Entrevista
            {
                id_oferente = Entrevista.id_oferente.Value,
                id_empleado_entrevistador = Entrevista.id_empleado_entrevistador.Value,
                fecha_entrevista = Entrevista.fecha_entrevista.Value,
                hora_entrevista = Entrevista.hora_entrevista.Value
            };

            int idEntrevista = _repository.Crear(entrevista);
            entrevista.id_entrevista = idEntrevista;

            _bitacoraService.RegistrarInsert(
                idUsuario,
                "Entrevista",
                new
                {
                    entrevista.id_entrevista,
                    entrevista.id_oferente,
                    entrevista.id_empleado_entrevistador,
                    entrevista.fecha_entrevista,
                    entrevista.hora_entrevista,
                    estado = "PENDIENTE"
                }
            );

            TempData["Exito"] = "Entrevista agendada correctamente.";
            return RedirectToPage("Index");
        }

        private void CargarDropdowns()
        {
            Entrevista.OferentesDisponibles = _repository.ListarOferentes();
            Entrevista.EmpleadosDisponibles = _repository.ListarEmpleados();
        }
    }
}