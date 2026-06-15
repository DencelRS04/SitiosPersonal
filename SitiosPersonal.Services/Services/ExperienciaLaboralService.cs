using SitiosPersonal.Entities.Models;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Exceptions;
using System.Text.RegularExpressions;

namespace SitiosPersonal.Services.Services
{
    public class ExperienciaLaboralService : ExperienciaLaboralRepository
    {
        private readonly BitacoraService _bitacoraService;

        private static readonly Regex SoloLetrasNumerosEspaciosRegex = new(
            @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9\s]+$",
            RegexOptions.Compiled);

        public ExperienciaLaboralService(
            DbContext context,
            BitacoraService bitacoraService) : base(context)
        {
            _bitacoraService = bitacoraService;
        }

        public int CrearExperiencia(
            ExperienciaLaboral experiencia,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarExperiencia(experiencia);

                int idExperiencia = Crear(experiencia);
                experiencia.id_experiencia = idExperiencia;

                _bitacoraService.RegistrarInsert(
                    idUsuarioEjecuta,
                    "ExperienciaLaboral",
                    new
                    {
                        experiencia.id_experiencia,
                        experiencia.id_oferente,
                        experiencia.empresa,
                        experiencia.puesto,
                        experiencia.fecha_inicio,
                        experiencia.fecha_fin
                    });

                return idExperiencia;
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "ExperienciaLaboral", ex.Message);
                throw;
            }
        }

        public void ActualizarExperiencia(
            ExperienciaLaboral experienciaAnterior,
            ExperienciaLaboral experienciaActual,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(experienciaActual.id_experiencia, "La experiencia laboral indicada no es válida.");
                ValidarExperiencia(experienciaActual);

                Actualizar(experienciaActual);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "ExperienciaLaboral",
                    new
                    {
                        experienciaAnterior.id_experiencia,
                        experienciaAnterior.id_oferente,
                        experienciaAnterior.empresa,
                        experienciaAnterior.puesto,
                        experienciaAnterior.fecha_inicio,
                        experienciaAnterior.fecha_fin
                    },
                    new
                    {
                        experienciaActual.id_experiencia,
                        experienciaActual.id_oferente,
                        experienciaActual.empresa,
                        experienciaActual.puesto,
                        experienciaActual.fecha_inicio,
                        experienciaActual.fecha_fin
                    });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "ExperienciaLaboral", ex.Message);
                throw;
            }
        }

        public bool EliminarExperiencia(
            int idExperiencia,
            int? idUsuarioEjecuta,
            out string? mensajeError)
        {
            mensajeError = null;

            try
            {
                ValidarId(idExperiencia, "La experiencia laboral indicada no es válida.");

                var experiencia = ObtenerPorId(idExperiencia);
                if (experiencia == null)
                {
                    mensajeError = "El registro no existe.";
                    return false;
                }

                Eliminar(idExperiencia);

                _bitacoraService.RegistrarDelete(
                    idUsuarioEjecuta,
                    "ExperienciaLaboral",
                    new
                    {
                        experiencia.id_experiencia,
                        experiencia.id_oferente,
                        experiencia.empresa,
                        experiencia.puesto
                    });

                return true;
            }
            catch (ValidacionException ex)
            {
                mensajeError = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "ExperienciaLaboral", ex.Message);
                throw;
            }
        }

        private static void ValidarExperiencia(ExperienciaLaboral experiencia)
        {
            if (experiencia == null)
                throw new ValidacionException("Debe indicar la información de la experiencia laboral.");

            if (experiencia.id_oferente <= 0)
                throw new ValidacionException("El oferente asociado no es válido.");

            experiencia.empresa = NormalizarTexto(experiencia.empresa);
            if (string.IsNullOrWhiteSpace(experiencia.empresa))
                throw new ValidacionException("El nombre de la empresa es obligatorio.");
            if (experiencia.empresa.Length > 100)
                throw new ValidacionException("El nombre de la empresa no puede superar los 100 caracteres.");
            if (!SoloLetrasNumerosEspaciosRegex.IsMatch(experiencia.empresa))
                throw new ValidacionException("El nombre de la empresa solo puede contener letras, números y espacios.");

            experiencia.puesto = NormalizarTexto(experiencia.puesto);
            if (string.IsNullOrWhiteSpace(experiencia.puesto))
                throw new ValidacionException("El puesto desempeñado es obligatorio.");
            if (experiencia.puesto.Length > 100)
                throw new ValidacionException("El puesto desempeñado no puede superar los 100 caracteres.");
            if (!SoloLetrasNumerosEspaciosRegex.IsMatch(experiencia.puesto))
                throw new ValidacionException("El puesto desempeñado solo puede contener letras, números y espacios.");

            if (experiencia.fecha_inicio == default)
                throw new ValidacionException("La fecha de inicio es obligatoria.");

            if (experiencia.fecha_fin == default)
                throw new ValidacionException("La fecha de fin es obligatoria.");

            if (experiencia.fecha_fin < experiencia.fecha_inicio)
                throw new ValidacionException("La fecha de fin debe ser mayor o igual a la fecha de inicio.");
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
