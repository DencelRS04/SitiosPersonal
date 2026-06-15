using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Pages.General.CargaUbicacion
{
    public class IndexModel : PageModel
    {
        private readonly UbicacionService _ubicacionService;
        private readonly BitacoraService _bitacoraService;
        private readonly PermisosService _permisosRepository;

        public IndexModel(UbicacionService ubicacionService, BitacoraService bitacoraService, PermisosService permisosRepository)
        {
            _ubicacionService = ubicacionService;
            _bitacoraService = bitacoraService;
            _permisosRepository = permisosRepository;
        }

        public IActionResult OnGet()
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            return Page();
        }

        public IActionResult OnPost(IFormFile archivo)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            // Validación de presentación: archivo presente y con extensión .csv
            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo válido.";
                return RedirectToPage("Index");
            }

            var extension = System.IO.Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (extension != ".csv")
            {
                TempData["Error"] = "Solo se aceptan archivos con extensión .csv.";
                return RedirectToPage("Index");
            }

            try
            {
                // Lectura del archivo (presentación)
                var lineas = new List<string>();
                using (var reader = new System.IO.StreamReader(archivo.OpenReadStream()))
                {
                    while (!reader.EndOfStream)
                        lineas.Add(reader.ReadLine() ?? "");
                }

                // Validación en la capa de negocio
                var validacion = _ubicacionService.ValidarArchivo(lineas);
                if (!validacion.EsValido)
                {
                    TempData["Error"] = validacion.Error;
                    return RedirectToPage("Index");
                }

                int provincias = 0, cantones = 0, distritos = 0;

                var provinciaMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                var cantonMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                foreach (var p in _ubicacionService.ListarProvincias())
                    provinciaMap[p.nombre] = p.id_provincia;

                foreach (var c in _ubicacionService.ListarCantones())
                    cantonMap[$"{c.nombre_provincia}|{c.nombre}"] = c.id_canton;

                int nextProvinciaId = provinciaMap.Count > 0 ? provinciaMap.Values.Max() + 1 : 1;
                int nextCantonId = cantonMap.Count > 0 ? cantonMap.Values.Max() + 1 : 1;
                int nextDistritoId = 1;

                var distritosExistentes = _ubicacionService.ListarDistritos().ToList();
                if (distritosExistentes.Count > 0)
                    nextDistritoId = ((IEnumerable<dynamic>)distritosExistentes).Max(d => (int)d.id_distrito) + 1;

                foreach (var (nombreProvincia, nombreCanton, nombreDistrito) in validacion.Lineas)
                {
                    if (!provinciaMap.TryGetValue(nombreProvincia, out int idProvincia))
                    {
                        idProvincia = nextProvinciaId++;
                        provinciaMap[nombreProvincia] = idProvincia;
                        _ubicacionService.UpsertProvincia(new Provincia { id_provincia = idProvincia, nombre = nombreProvincia });
                        provincias++;
                    }

                    string cantonKey = $"{nombreProvincia}|{nombreCanton}";
                    if (!cantonMap.TryGetValue(cantonKey, out int idCanton))
                    {
                        idCanton = nextCantonId++;
                        cantonMap[cantonKey] = idCanton;
                        _ubicacionService.UpsertCanton(new Canton { id_canton = idCanton, id_provincia = idProvincia, nombre = nombreCanton });
                        cantones++;
                    }

                    _ubicacionService.UpsertDistrito(new Distrito { id_distrito = nextDistritoId++, id_canton = idCanton, nombre = nombreDistrito });
                    distritos++;
                }

                _bitacoraService.RegistrarInsert(idUsuario, "CargaUbicacion",
                    new { descripcion = "Se realizó la carga de información de ubicación." });

                TempData["Exito"] = $"Carga completada: {provincias} provincia(s), {cantones} cantón(es), {distritos} distrito(s) procesados.";
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuario, "CargaUbicacion", ex.Message);
                TempData["Error"] = "Ocurrió un error al procesar el archivo: " + ex.Message;
            }

            return RedirectToPage("Index");
        }

        private IActionResult? ValidarAcceso()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                TempData["Mensaje"] = Request.Cookies.ContainsKey("SesionIniciada")
                    ? "La sesión ha expirado. Por favor inicie sesión nuevamente."
                    : "Por favor inicie sesión para utilizar el sistema";
                return RedirectToPage("/Login/Index");
            }

            var rutasPermitidas = _permisosRepository.ObtenerRutasPermitidas(idUsuario.Value);
            bool tienePermiso = rutasPermitidas.Any(ruta =>
                !string.IsNullOrWhiteSpace(ruta) &&
                Request.Path.Value!.StartsWith(ruta, StringComparison.OrdinalIgnoreCase));

            if (!tienePermiso)
            {
                TempData["Error"] = "No tiene permisos para acceder a esta pantalla.";
                return RedirectToPage("/Home/Index");
            }

            return null;
        }
    }
}
