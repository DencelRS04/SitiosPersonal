using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Services.Services
{
    public class ResultadoValidacionUbicacion
    {
        public bool EsValido { get; set; }
        public string Error { get; set; } = "";
        public List<(string provincia, string canton, string distrito)> Lineas { get; set; } = new();
    }

    public class UbicacionService : UbicacionRepository
    {
        public UbicacionService(DbContext context) : base(context) { }

        // Validación en la capa de negocio del contenido del archivo de ubicación
        public ResultadoValidacionUbicacion ValidarArchivo(List<string> lineas)
        {
            var resultado = new ResultadoValidacionUbicacion();

            if (lineas == null || lineas.Count == 0)
            {
                resultado.Error = "El archivo no contiene datos.";
                return resultado;
            }

            var encabezado = lineas[0]?.Trim();
            if (encabezado == null || !encabezado.Equals("Provincia,Canton,Distrito", StringComparison.OrdinalIgnoreCase))
            {
                resultado.Error = "El archivo no tiene el encabezado requerido. La primera línea debe ser exactamente: Provincia,Canton,Distrito";
                return resultado;
            }

            for (int i = 1; i < lineas.Count; i++)
            {
                int numeroLinea = i + 1;
                var linea = lineas[i];

                if (string.IsNullOrWhiteSpace(linea)) continue;

                if (linea.Contains(';') || linea.Contains('\t') || linea.Contains('|'))
                {
                    resultado.Error = $"Línea {numeroLinea}: se detectó un delimitador no permitido. Use únicamente coma (,) como separador.";
                    return resultado;
                }

                var partes = linea.Split(',');
                if (partes.Length != 3)
                {
                    resultado.Error = $"Línea {numeroLinea}: se encontraron {partes.Length} columna(s). El archivo debe tener exactamente 3 columnas: Provincia,Canton,Distrito.";
                    return resultado;
                }

                string nombreProvincia = partes[0].Trim();
                string nombreCanton = partes[1].Trim();
                string nombreDistrito = partes[2].Trim();

                if (string.IsNullOrWhiteSpace(nombreProvincia))
                {
                    resultado.Error = $"Línea {numeroLinea}: el campo Provincia está vacío.";
                    return resultado;
                }
                if (string.IsNullOrWhiteSpace(nombreCanton))
                {
                    resultado.Error = $"Línea {numeroLinea}: el campo Canton está vacío.";
                    return resultado;
                }
                if (string.IsNullOrWhiteSpace(nombreDistrito))
                {
                    resultado.Error = $"Línea {numeroLinea}: el campo Distrito está vacío.";
                    return resultado;
                }

                resultado.Lineas.Add((nombreProvincia, nombreCanton, nombreDistrito));
            }

            if (resultado.Lineas.Count == 0)
            {
                resultado.Error = "El archivo no contiene datos de ubicación.";
                return resultado;
            }

            resultado.EsValido = true;
            return resultado;
        }
    }
}
