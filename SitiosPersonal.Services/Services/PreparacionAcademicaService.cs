using SitiosPersonal.Entities.Models;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Exceptions;
using System.Text.RegularExpressions;

namespace SitiosPersonal.Services.Services
{
    public class PreparacionAcademicaService : PreparacionAcademicaRepository
    {
        private readonly BitacoraService _bitacoraService;

        private static readonly Regex SoloLetrasEspaciosRegex = new(
            @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$",
            RegexOptions.Compiled);

        public PreparacionAcademicaService(
            DbContext context,
            BitacoraService bitacoraService) : base(context)
        {
            _bitacoraService = bitacoraService;
        }

        public int CrearPreparacion(
            PreparacionAcademica preparacion,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarPreparacion(preparacion);

                int idPreparacion = Crear(preparacion);
                preparacion.id_preparacion = idPreparacion;

                _bitacoraService.RegistrarInsert(
                    idUsuarioEjecuta,
                    "PreparacionAcademica",
                    new
                    {
                        preparacion.id_preparacion,
                        preparacion.id_oferente,
                        preparacion.id_institucion,
                        preparacion.titulo,
                        preparacion.fecha_inicio,
                        preparacion.fecha_fin
                    });

                return idPreparacion;
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "PreparacionAcademica", ex.Message);
                throw;
            }
        }

        public void ActualizarPreparacion(
            PreparacionAcademica preparacionAnterior,
            PreparacionAcademica preparacionActual,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(preparacionActual.id_preparacion, "La preparación académica indicada no es válida.");
                ValidarPreparacion(preparacionActual);

                Actualizar(preparacionActual);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "PreparacionAcademica",
                    new
                    {
                        preparacionAnterior.id_preparacion,
                        preparacionAnterior.id_oferente,
                        preparacionAnterior.id_institucion,
                        preparacionAnterior.titulo,
                        preparacionAnterior.fecha_inicio,
                        preparacionAnterior.fecha_fin
                    },
                    new
                    {
                        preparacionActual.id_preparacion,
                        preparacionActual.id_oferente,
                        preparacionActual.id_institucion,
                        preparacionActual.titulo,
                        preparacionActual.fecha_inicio,
                        preparacionActual.fecha_fin
                    });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "PreparacionAcademica", ex.Message);
                throw;
            }
        }

        public bool EliminarPreparacion(
            int idPreparacion,
            int? idUsuarioEjecuta,
            out string? mensajeError)
        {
            mensajeError = null;

            try
            {
                ValidarId(idPreparacion, "La preparación académica indicada no es válida.");

                var preparacion = ObtenerPorId(idPreparacion);
                if (preparacion == null)
                {
                    mensajeError = "El registro no existe.";
                    return false;
                }

                Eliminar(idPreparacion);

                _bitacoraService.RegistrarDelete(
                    idUsuarioEjecuta,
                    "PreparacionAcademica",
                    new
                    {
                        preparacion.id_preparacion,
                        preparacion.id_oferente,
                        preparacion.id_institucion,
                        preparacion.titulo
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
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "PreparacionAcademica", ex.Message);
                throw;
            }
        }

        private static void ValidarPreparacion(PreparacionAcademica preparacion)
        {
            if (preparacion == null)
                throw new ValidacionException("Debe indicar la información de la preparación académica.");

            if (preparacion.id_oferente <= 0)
                throw new ValidacionException("El oferente asociado no es válido.");

            if (preparacion.id_institucion <= 0)
                throw new ValidacionException("Debe seleccionar una institución educativa válida.");

            preparacion.titulo = NormalizarTexto(preparacion.titulo);
            if (string.IsNullOrWhiteSpace(preparacion.titulo))
                throw new ValidacionException("El nombre del título es obligatorio.");
            if (preparacion.titulo.Length > 100)
                throw new ValidacionException("El título no puede superar los 100 caracteres.");
            if (!SoloLetrasEspaciosRegex.IsMatch(preparacion.titulo))
                throw new ValidacionException("El título solo puede contener letras y espacios.");

            if (preparacion.fecha_inicio == default)
                throw new ValidacionException("La fecha de inicio es obligatoria.");

            if (preparacion.fecha_fin == default)
                throw new ValidacionException("La fecha de fin es obligatoria.");

            if (preparacion.fecha_fin < preparacion.fecha_inicio)
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
