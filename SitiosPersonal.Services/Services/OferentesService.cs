using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Exceptions;
using System.Text.RegularExpressions;

namespace SitiosPersonal.Services.Services
{
    public class OferentesService : OferentesRepository
    {
        private readonly BitacoraService _bitacoraService;

        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled);

        private static readonly string[] TiposIdentificacionValidos =
            { "CEDULA", "DIMEX", "PASAPORTE" };

        public OferentesService(
            DbContext context,
            BitacoraService bitacoraService) : base(context)
        {
            _bitacoraService = bitacoraService;
        }

        public OferentesListaViewModel ObtenerListado(
            int pagina,
            int cantidadPorPagina,
            int? idUsuarioEjecuta)
        {
            if (pagina < 1) pagina = 1;
            if (cantidadPorPagina < 1) cantidadPorPagina = 10;

            try
            {
                var lista = new OferentesListaViewModel
                {
                    Pagina = pagina,
                    CantidadPorPagina = cantidadPorPagina,
                    TotalRegistros = Contar(),
                    Oferentes = ListarPaginado(pagina, cantidadPorPagina)
                };

                _bitacoraService.RegistrarConsulta(idUsuarioEjecuta, "Oferentes");

                return lista;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Oferente", ex.Message);
                throw;
            }
        }

        public int CrearOferente(
            Oferente oferente,
            List<string> correos,
            List<string> telefonos,
            List<int> concursos,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarOferente(oferente, correos, telefonos);

                if (ExisteIdentificacion(oferente.identificacion))
                    throw new ValidacionException("El número de identificación ya está registrado en el sistema.");

                int idOferente = Crear(oferente, correos, telefonos, concursos);
                oferente.id_oferente = idOferente;

                _bitacoraService.RegistrarInsert(
                    idUsuarioEjecuta,
                    "Oferente",
                    new
                    {
                        oferente.id_oferente,
                        oferente.identificacion,
                        oferente.tipo_identificacion,
                        oferente.nombre_completo,
                        correos,
                        telefonos,
                        concursos
                    });

                return idOferente;
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Oferente", ex.Message);
                throw;
            }
        }

        public void ActualizarOferente(
            Oferente oferente,
            List<string> correos,
            List<string> telefonos,
            List<int> concursos,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(oferente.id_oferente, "El oferente indicado no es válido.");
                ValidarOferente(oferente, correos, telefonos);

                if (ExisteIdentificacion(oferente.identificacion, oferente.id_oferente))
                    throw new ValidacionException("El número de identificación ya está registrado en el sistema.");

                var anterior = ObtenerPorId(oferente.id_oferente);
                if (anterior == null)
                    throw new ValidacionException("El oferente no existe.");

                var correosAnteriores = ObtenerCorreos(oferente.id_oferente);
                var telefonosAnteriores = ObtenerTelefonos(oferente.id_oferente);
                var concursosAnteriores = ObtenerConcursos(oferente.id_oferente);

                Actualizar(oferente, correos, telefonos, concursos);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "Oferente",
                    new
                    {
                        anterior.id_oferente,
                        anterior.identificacion,
                        anterior.tipo_identificacion,
                        anterior.nombre_completo,
                        correos = correosAnteriores,
                        telefonos = telefonosAnteriores,
                        concursos = concursosAnteriores
                    },
                    new
                    {
                        oferente.id_oferente,
                        oferente.identificacion,
                        oferente.tipo_identificacion,
                        oferente.nombre_completo,
                        correos,
                        telefonos,
                        concursos
                    });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Oferente", ex.Message);
                throw;
            }
        }

        public bool EliminarOferente(
            int idOferente,
            int? idUsuarioEjecuta,
            out string? mensajeError)
        {
            mensajeError = null;

            try
            {
                ValidarId(idOferente, "El oferente indicado no es válido.");

                var oferente = ObtenerPorId(idOferente);
                if (oferente == null)
                {
                    mensajeError = "El registro no existe.";
                    return false;
                }

                if (!PuedeEliminar(idOferente))
                {
                    mensajeError = "No se puede eliminar un oferente con datos relacionados.";
                    return false;
                }

                Eliminar(idOferente);

                _bitacoraService.RegistrarDelete(
                    idUsuarioEjecuta,
                    "Oferente",
                    new { oferente.id_oferente, oferente.identificacion, oferente.nombre_completo });

                return true;
            }
            catch (ValidacionException ex)
            {
                mensajeError = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Oferente", ex.Message);
                throw;
            }
        }

        private void ValidarOferente(Oferente oferente, List<string> correos, List<string> telefonos)
        {
            if (oferente == null)
                throw new ValidacionException("Debe indicar la información del oferente.");

            oferente.identificacion = NormalizarTexto(oferente.identificacion);
            if (string.IsNullOrWhiteSpace(oferente.identificacion))
                throw new ValidacionException("La identificación del oferente es obligatoria.");
            if (oferente.identificacion.Length > 30)
                throw new ValidacionException("La identificación no puede superar los 30 caracteres.");

            if (string.IsNullOrWhiteSpace(oferente.tipo_identificacion) ||
                !TiposIdentificacionValidos.Contains(oferente.tipo_identificacion))
                throw new ValidacionException("El tipo de identificación debe ser CEDULA, DIMEX o PASAPORTE.");

            oferente.nombre_completo = NormalizarTexto(oferente.nombre_completo);
            if (string.IsNullOrWhiteSpace(oferente.nombre_completo))
                throw new ValidacionException("El nombre completo del oferente es obligatorio.");
            if (oferente.nombre_completo.Length > 150)
                throw new ValidacionException("El nombre completo no puede superar los 150 caracteres.");

            if (oferente.fecha_nacimiento == default || oferente.fecha_nacimiento >= DateTime.Today)
                throw new ValidacionException("La fecha de nacimiento debe ser anterior a la fecha actual.");

            var correosValidos = correos?.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim()).ToList()
                                 ?? new List<string>();

            if (!correosValidos.Any())
                throw new ValidacionException("Debe ingresar al menos un correo electrónico.");

            foreach (var correo in correosValidos)
            {
                if (correo.Length > 150)
                    throw new ValidacionException($"El correo '{correo}' supera los 150 caracteres permitidos.");
                if (!EmailRegex.IsMatch(correo))
                    throw new ValidacionException($"El correo '{correo}' no tiene un formato válido.");
            }

            var telefonosValidos = telefonos?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList()
                                   ?? new List<string>();

            if (!telefonosValidos.Any())
                throw new ValidacionException("Debe ingresar al menos un teléfono de contacto.");

            foreach (var telefono in telefonosValidos)
            {
                if (telefono.Length > 20)
                    throw new ValidacionException($"El teléfono '{telefono}' supera los 20 caracteres permitidos.");
            }
        }

        private static void ValidarId(int id, string mensaje)
        {
            if (id <= 0)
                throw new ValidacionException(mensaje);
        }

        private static string NormalizarTexto(string? texto)
        {
            return string.IsNullOrWhiteSpace(texto)
                ? string.Empty
                : Regex.Replace(texto.Trim(), @"\s+", " ");
        }
    }
}
