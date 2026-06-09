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

        public IEnumerable<Provincia> Provincias { get; set; } = new List<Provincia>();
        public IEnumerable<dynamic> Cantones { get; set; } = new List<dynamic>();
        public IEnumerable<dynamic> Distritos { get; set; } = new List<dynamic>();

        public IActionResult OnGet()
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            CargarListas();
            return Page();
        }

        public IActionResult OnPost(IFormFile archivo)
        {
            var resultado = ValidarAcceso();
            if (resultado != null) return resultado;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo válido.";
                return RedirectToPage("Index");
            }

            // Validar extensión .csv únicamente
            var extension = System.IO.Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (extension != ".csv")
            {
                TempData["Error"] = "Solo se aceptan archivos con extensión .csv.";
                return RedirectToPage("Index");
            }

            try
            {
                using var reader = new System.IO.StreamReader(archivo.OpenReadStream());

                // --- Validar encabezado obligatorio exacto ---
                var encabezado = reader.ReadLine()?.Trim();
                if (encabezado == null || !encabezado.Equals("Provincia,Canton,Distrito", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "El archivo no tiene el encabezado requerido. La primera línea debe ser exactamente: Provincia,Canton,Distrito";
                    return RedirectToPage("Index");
                }

                // --- Leer todas las líneas y validar formato estricto antes de procesar ---
                var lineasValidas = new List<(string provincia, string canton, string distrito)>();
                int numeroLinea = 1; // empieza en 2 considerando el encabezado

                while (!reader.EndOfStream)
                {
                    numeroLinea++;
                    var linea = reader.ReadLine();

                    // Saltar líneas completamente vacías
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    // Detectar delimitadores no permitidos (punto y coma, tabulador, pipe)
                    if (linea.Contains(';') || linea.Contains('\t') || linea.Contains('|'))
                    {
                        TempData["Error"] = $"Línea {numeroLinea}: se detectó un delimitador no permitido. Use únicamente coma (,) como separador.";
                        return RedirectToPage("Index");
                    }

                    var partes = linea.Split(',');

                    // Debe tener exactamente 3 columnas
                    if (partes.Length != 3)
                    {
                        TempData["Error"] = $"Línea {numeroLinea}: se encontraron {partes.Length} columna(s). El archivo debe tener exactamente 3 columnas: Provincia,Canton,Distrito.";
                        return RedirectToPage("Index");
                    }

                    string nombreProvincia = partes[0].Trim();
                    string nombreCanton = partes[1].Trim();
                    string nombreDistrito = partes[2].Trim();

                    // Ningún campo puede estar vacío
                    if (string.IsNullOrWhiteSpace(nombreProvincia))
                    {
                        TempData["Error"] = $"Línea {numeroLinea}: el campo Provincia está vacío.";
                        return RedirectToPage("Index");
                    }
                    if (string.IsNullOrWhiteSpace(nombreCanton))
                    {
                        TempData["Error"] = $"Línea {numeroLinea}: el campo Canton está vacío.";
                        return RedirectToPage("Index");
                    }
                    if (string.IsNullOrWhiteSpace(nombreDistrito))
                    {
                        TempData["Error"] = $"Línea {numeroLinea}: el campo Distrito está vacío.";
                        return RedirectToPage("Index");
                    }

                    lineasValidas.Add((nombreProvincia, nombreCanton, nombreDistrito));
                }

                if (lineasValidas.Count == 0)
                {
                    TempData["Error"] = "El archivo no contiene datos de ubicación.";
                    return RedirectToPage("Index");
                }

                // --- Procesar registros validados ---
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

                foreach (var (nombreProvincia, nombreCanton, nombreDistrito) in lineasValidas)
                {
                    // Provincia
                    if (!provinciaMap.TryGetValue(nombreProvincia, out int idProvincia))
                    {
                        idProvincia = nextProvinciaId++;
                        provinciaMap[nombreProvincia] = idProvincia;
                        _ubicacionService.UpsertProvincia(new Provincia { id_provincia = idProvincia, nombre = nombreProvincia });
                        provincias++;
                    }

                    // Cantón
                    string cantonKey = $"{nombreProvincia}|{nombreCanton}";
                    if (!cantonMap.TryGetValue(cantonKey, out int idCanton))
                    {
                        idCanton = nextCantonId++;
                        cantonMap[cantonKey] = idCanton;
                        _ubicacionService.UpsertCanton(new Canton { id_canton = idCanton, id_provincia = idProvincia, nombre = nombreCanton });
                        cantones++;
                    }

                    // Distrito
                    _ubicacionService.UpsertDistrito(new Distrito { id_distrito = nextDistritoId++, id_canton = idCanton, nombre = nombreDistrito });
                    distritos++;
                }

                _bitacoraService.RegistrarInsert(idUsuario, "CargaUbicacion",
                    new { descripcion = "Se realizó la carga de información de ubicación.", provincias, cantones, distritos });

                TempData["Exito"] = $"Carga completada: {provincias} provincia(s), {cantones} cantón(es), {distritos} distrito(s) procesados.";
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuario, "CargaUbicacion", ex.Message);
                TempData["Error"] = "Ocurrió un error al procesar el archivo: " + ex.Message;
            }

            return RedirectToPage("Index");
        }

        private void CargarListas()
        {
            Provincias = _ubicacionService.ListarProvincias();
            Cantones = _ubicacionService.ListarCantones();
            Distritos = _ubicacionService.ListarDistritos();
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