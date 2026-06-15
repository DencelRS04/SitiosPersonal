using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Exceptions;

namespace SitiosPersonal.Services.Services
{
    public class EntrevistasService : EntrevistasRepository
    {
        private readonly BitacoraService _bitacoraService;

        public EntrevistasService(
            DbContext context,
            BitacoraService bitacoraService) : base(context)
        {
            _bitacoraService = bitacoraService;
        }

        public EntrevistasListaViewModel ObtenerListado(
            int pagina,
            int cantidadPorPagina,
            int? idUsuarioEjecuta)
        {
            if (pagina < 1) pagina = 1;
            if (cantidadPorPagina < 1) cantidadPorPagina = 10;

            try
            {
                var lista = new EntrevistasListaViewModel
                {
                    Pagina = pagina,
                    CantidadPorPagina = cantidadPorPagina,
                    TotalRegistros = Contar(),
                    Entrevistas = ListarPaginado(pagina, cantidadPorPagina)
                };

                _bitacoraService.RegistrarConsulta(idUsuarioEjecuta, "Entrevistas");

                return lista;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Entrevista", ex.Message);
                throw;
            }
        }

        public int CrearEntrevista(
            Entrevista entrevista,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarEntrevista(entrevista);

                int idEntrevista = Crear(entrevista);
                entrevista.id_entrevista = idEntrevista;

                _bitacoraService.RegistrarInsert(
                    idUsuarioEjecuta,
                    "Entrevista",
                    new
                    {
                        entrevista.id_entrevista,
                        entrevista.id_oferente,
                        entrevista.id_empleado_entrevistador,
                        entrevista.fecha_entrevista,
                        entrevista.hora_entrevista,
                        estado = "PENDIENTE"
                    });

                return idEntrevista;
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Entrevista", ex.Message);
                throw;
            }
        }

        public void ActualizarEntrevista(
            Entrevista entrevistaAnterior,
            Entrevista entrevistaActual,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(entrevistaActual.id_entrevista, "La entrevista indicada no es válida.");
                ValidarEntrevista(entrevistaActual);

                Actualizar(entrevistaActual);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "Entrevista",
                    new
                    {
                        entrevistaAnterior.id_entrevista,
                        entrevistaAnterior.id_oferente,
                        entrevistaAnterior.id_empleado_entrevistador,
                        entrevistaAnterior.fecha_entrevista,
                        entrevistaAnterior.hora_entrevista
                    },
                    new
                    {
                        entrevistaActual.id_entrevista,
                        entrevistaActual.id_oferente,
                        entrevistaActual.id_empleado_entrevistador,
                        entrevistaActual.fecha_entrevista,
                        entrevistaActual.hora_entrevista
                    });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Entrevista", ex.Message);
                throw;
            }
        }

        public void MarcarEntrevistaRealizada(
            int idEntrevista,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(idEntrevista, "La entrevista indicada no es válida.");

                var entrevista = ObtenerPorId(idEntrevista);
                if (entrevista == null)
                    throw new ValidacionException("La entrevista no existe.");

                if (entrevista.estado == "REALIZADA")
                    throw new ValidacionException("La entrevista ya fue marcada como realizada.");

                MarcarRealizada(idEntrevista);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "Entrevista",
                    new { entrevista.id_entrevista, entrevista.id_oferente, estado = entrevista.estado },
                    new { entrevista.id_entrevista, entrevista.id_oferente, estado = "REALIZADA" });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Entrevista", ex.Message);
                throw;
            }
        }

        public bool EliminarEntrevista(
            int idEntrevista,
            int? idUsuarioEjecuta,
            out string? mensajeError)
        {
            mensajeError = null;

            try
            {
                ValidarId(idEntrevista, "La entrevista indicada no es válida.");

                var entrevista = ObtenerPorId(idEntrevista);
                if (entrevista == null)
                {
                    mensajeError = "El registro no existe.";
                    return false;
                }

                Eliminar(idEntrevista);

                _bitacoraService.RegistrarDelete(
                    idUsuarioEjecuta,
                    "Entrevista",
                    new
                    {
                        entrevista.id_entrevista,
                        entrevista.id_oferente,
                        entrevista.id_empleado_entrevistador,
                        entrevista.estado
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
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Entrevista", ex.Message);
                throw;
            }
        }

        private static void ValidarEntrevista(Entrevista entrevista)
        {
            if (entrevista == null)
                throw new ValidacionException("Debe indicar la información de la entrevista.");

            if (entrevista.id_oferente <= 0)
                throw new ValidacionException("Debe seleccionar un oferente válido.");

            if (entrevista.id_empleado_entrevistador <= 0)
                throw new ValidacionException("Debe seleccionar un empleado entrevistador válido.");

            if (entrevista.fecha_entrevista == default)
                throw new ValidacionException("La fecha de la entrevista es obligatoria.");

            if (entrevista.hora_entrevista == default)
                throw new ValidacionException("La hora de la entrevista es obligatoria.");
        }

        private static void ValidarId(int id, string mensaje)
        {
            if (id <= 0)
                throw new ValidacionException(mensaje);
        }
    }
}
