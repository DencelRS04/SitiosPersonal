using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Services.Services;
using SitiosPersonal.Services.Services;
using System.Text.RegularExpressions;

namespace SitiosPersonal.Pages.Oferentes
{
    public class CrearModel : PageModel
    {
        private readonly OferentesService _repository;
        private readonly BitacoraService _bitacoraService;

        public CrearModel(OferentesService repository, BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public OferenteViewModel Oferente { get; set; } = new OferenteViewModel();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            Oferente.ConcursosDisponibles = _repository.ListarConcursos();
            return Page();
        }

        public IActionResult OnPost()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (!ModelState.IsValid || !ValidarCamposMultiples())
            {
                Oferente.ConcursosDisponibles = _repository.ListarConcursos();
                return Page();
            }

            if (_repository.ExisteIdentificacion(Oferente.identificacion))
            {
                TempData["Error"] = "El número de identificación ya está registrado en el sistema.";
                Oferente.ConcursosDisponibles = _repository.ListarConcursos();
                return Page();
            }

            var oferente = new Oferente
            {
                identificacion = Oferente.identificacion.Trim(),
                tipo_identificacion = Oferente.tipo_identificacion,
                nombre_completo = Oferente.nombre_completo.Trim(),
                fecha_nacimiento = Oferente.fecha_nacimiento!.Value
            };

            var correos = Oferente.Correos.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim()).ToList();
            var telefonos = Oferente.Telefonos.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).ToList();

            int idOferente = _repository.Crear(oferente, correos, telefonos, Oferente.ConcursosSeleccionados);
            oferente.id_oferente = idOferente;

            _bitacoraService.RegistrarInsert(
                idUsuario,
                "Oferente",
                new
                {
                    oferente.id_oferente,
                    oferente.identificacion,
                    oferente.tipo_identificacion,
                    oferente.nombre_completo,
                    correos,
                    telefonos,
                    concursos = Oferente.ConcursosSeleccionados
                }
            );

            TempData["Exito"] = "El oferente ha sido registrado correctamente.";
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
