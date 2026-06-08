using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.Entrevistas
{
    public class EditarModel : PageModel
    {
        private readonly EntrevistasService _repository;
        private readonly OferentesService _oferentesRepository;
        private readonly BitacoraService _bitacoraService;

        public EditarModel(
            EntrevistasService repository,
            OferentesService oferentesRepository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _oferentesRepository = oferentesRepository;
            _bitacoraService = bitacoraService;
            Entrevista = new EntrevistaViewModel();
        }

        [BindProperty]
        public EntrevistaViewModel Entrevista { get; set; }

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var entrevista = _repository.ObtenerPorId(id);
            if (entrevista == null)
            {
                return RedirectToPage("Index");
            }

            var oferente = _oferentesRepository.ObtenerPorId(entrevista.id_oferente);

            Entrevista = new EntrevistaViewModel
            {
                id_entrevista = entrevista.id_entrevista,
                id_oferente = entrevista.id_oferente,
                NombreOferente = oferente != null ? oferente.nombre_completo : "",
                id_empleado_entrevistador = entrevista.id_empleado_entrevistador,
                fecha_entrevista = entrevista.fecha_entrevista,
                hora_entrevista = entrevista.hora_entrevista,
                estado = entrevista.estado,
                EmpleadosDisponibles = _repository.ListarEmpleados()
            };

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            // Quitar validación de id_oferente: no se cambia en editar
            ModelState.Remove("Entrevista.id_oferente");

            if (!ModelState.IsValid)
            {
                // Recargar datos de solo lectura
                var ent = _repository.ObtenerPorId(id);
                if (ent != null)
                {
                    var oferente = _oferentesRepository.ObtenerPorId(ent.id_oferente);
                    Entrevista.NombreOferente = oferente != null ? oferente.nombre_completo : "";
                }
                Entrevista.EmpleadosDisponibles = _repository.ListarEmpleados();
                return Page();
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var anterior = _repository.ObtenerPorId(id);

            if (anterior == null)
            {
                return RedirectToPage("Index");
            }

            var actualizado = new Entities.Models.Entrevista
            {
                id_entrevista = id,
                id_oferente = anterior.id_oferente,
                id_empleado_entrevistador = Entrevista.id_empleado_entrevistador.Value,
                fecha_entrevista = Entrevista.fecha_entrevista.Value,
                hora_entrevista = Entrevista.hora_entrevista.Value
            };

            _repository.Actualizar(actualizado);

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Entrevista",
                new
                {
                    anterior.id_entrevista,
                    anterior.id_oferente,
                    anterior.id_empleado_entrevistador,
                    anterior.fecha_entrevista,
                    anterior.hora_entrevista
                },
                new
                {
                    actualizado.id_entrevista,
                    actualizado.id_oferente,
                    actualizado.id_empleado_entrevistador,
                    actualizado.fecha_entrevista,
                    actualizado.hora_entrevista
                }
            );

            TempData["Exito"] = "Datos de la entrevista actualizados correctamente.";
            return RedirectToPage("Index");
        }
    }
}