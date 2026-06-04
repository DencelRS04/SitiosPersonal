using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Services;
using System.Text.RegularExpressions;

namespace SitiosPersonal.Pages.Oferentes
{
    public class EditarModel : PageModel
    {
        private readonly OferentesRepository _repository;
        private readonly BitacoraService _bitacoraService;

        public EditarModel(OferentesRepository repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public OferenteViewModel Oferente { get; set; } = new OferenteViewModel();

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var oferente = _repository.ObtenerPorId(id);
            if (oferente == null)
            {
                return RedirectToPage("Index");
            }

            Oferente = new OferenteViewModel
            {
                id_oferente         = oferente.id_oferente,
                identificacion      = oferente.identificacion,
                tipo_identificacion = oferente.tipo_identificacion,
                nombre_completo     = oferente.nombre_completo,
                fecha_nacimiento    = oferente.fecha_nacimiento,
                Correos             = _repository.ObtenerCorreos(id),
                Telefonos           = _repository.ObtenerTelefonos(id),
                ConcursosSeleccionados = _repository.ObtenerConcursos(id),
                ConcursosDisponibles   = _repository.ListarConcursos()
            };

            if (!Oferente.Correos.Any())   Oferente.Correos.Add("");
            if (!Oferente.Telefonos.Any()) Oferente.Telefonos.Add("");

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (!ModelState.IsValid || !ValidarCamposMultiples())
            {
                Oferente.ConcursosDisponibles = _repository.ListarConcursos();
                return Page();
            }

            if (_repository.ExisteIdentificacion(Oferente.identificacion, id))
            {
                TempData["Error"] = "El número de identificación ya está registrado en el sistema.";
                Oferente.ConcursosDisponibles = _repository.ListarConcursos();
                return Page();
            }

            var oferenteAnterior    = _repository.ObtenerPorId(id);
            var correosAnteriores   = _repository.ObtenerCorreos(id);
            var telefonosAnteriores = _repository.ObtenerTelefonos(id);
            var concursosAnteriores = _repository.ObtenerConcursos(id);

            if (oferenteAnterior == null)
            {
                return RedirectToPage("Index");
            }

            var oferenteActual = new Oferente
            {
                id_oferente         = id,
                identificacion      = Oferente.identificacion.Trim(),
                tipo_identificacion = Oferente.tipo_identificacion,
                nombre_completo     = Oferente.nombre_completo.Trim(),
                fecha_nacimiento    = Oferente.fecha_nacimiento!.Value
            };

            var correos   = Oferente.Correos.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim()).ToList();
            var telefonos = Oferente.Telefonos.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).ToList();

            _repository.Actualizar(oferenteActual, correos, telefonos, Oferente.ConcursosSeleccionados);

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Oferente",
                new
                {
                    oferenteAnterior.id_oferente,
                    oferenteAnterior.identificacion,
                    oferenteAnterior.tipo_identificacion,
                    oferenteAnterior.nombre_completo,
                    correos   = correosAnteriores,
                    telefonos = telefonosAnteriores,
                    concursos = concursosAnteriores
                },
                new
                {
                    oferenteActual.id_oferente,
                    oferenteActual.identificacion,
                    oferenteActual.tipo_identificacion,
                    oferenteActual.nombre_completo,
                    correos,
                    telefonos,
                    concursos = Oferente.ConcursosSeleccionados
                }
            );

            TempData["Exito"] = "El oferente ha sido actualizado correctamente.";
            return RedirectToPage("Index");
        }

        private bool ValidarCamposMultiples()
        {
            bool valido = true;

            var correosValidos = Oferente.Correos?.Where(c => !string.IsNullOrWhiteSpace(c)).ToList()
                                 ?? new List<string>();

            if (!correosValidos.Any())
            {
                ModelState.AddModelError("", "Debe ingresar al menos un correo electrónico.");
                valido = false;
            }
            else
            {
                var regexEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                foreach (var correo in correosValidos)
                {
                    if (!regexEmail.IsMatch(correo))
                    {
                        ModelState.AddModelError("", $"El correo '{correo}' no tiene un formato válido.");
                        valido = false;
                    }
                }
            }

            var telefonosValidos = Oferente.Telefonos?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList()
                                   ?? new List<string>();

            if (!telefonosValidos.Any())
            {
                ModelState.AddModelError("", "Debe ingresar al menos un teléfono de contacto.");
                valido = false;
            }

            if (Oferente.ConcursosSeleccionados == null || !Oferente.ConcursosSeleccionados.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un concurso.");
                valido = false;
            }

            return valido;
        }
    }
}
