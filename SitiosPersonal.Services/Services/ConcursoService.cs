using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Exceptions;
using System.Text.RegularExpressions;

namespace SitiosPersonal.Services.Services
{
    public class ConcursosService : ConcursosRepository
    {
        private readonly BitacoraService _bitacoraService;

        private static readonly string[] EstadosValidos = { "VIGENTE", "VENCIDO" };

        public ConcursosService(
            DbContext context,
            BitacoraService bitacoraService) : base(context)
        {
            _bitacoraService = bitacoraService;
        }

        public ConcursosListaViewModel ObtenerListado(
            int pagina,
            int cantidadPorPagina,
            int? idUsuarioEjecuta)
        {
            if (pagina < 1) pagina = 1;
            if (cantidadPorPagina < 1) cantidadPorPagina = 10;

            try
            {
                var lista = new ConcursosListaViewModel
                {
                    Pagina = pagina,
                    CantidadPorPagina = cantidadPorPagina,
                    TotalRegistros = Contar(),
                    Concursos = ListarPaginado(pagina, cantidadPorPagina)
                };

                _bitacoraService.RegistrarConsulta(idUsuarioEjecuta, "Concursos");

                return lista;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Concurso", ex.Message);
                throw;
            }
        }

        public int CrearConcurso(
            Concurso concurso,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarConcurso(concurso);

                if (ExisteCodigo(concurso.codigo))
                    throw new ValidacionException("El código del concurso ya está registrado en el sistema.");

                int idConcurso = Crear(concurso);
                concurso.id_concurso = idConcurso;

                _bitacoraService.RegistrarInsert(
                    idUsuarioEjecuta,
                    "Concurso",
                    new { concurso.id_concurso, concurso.codigo, concurso.nombre, concurso.estado });

                return idConcurso;
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Concurso", ex.Message);
                throw;
            }
        }

        public void ActualizarConcurso(
            Concurso concurso,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(concurso.id_concurso, "El concurso indicado no es válido.");
                ValidarConcurso(concurso);

                if (ExisteCodigo(concurso.codigo, concurso.id_concurso))
                    throw new ValidacionException("El código del concurso ya está registrado en el sistema.");

                var anterior = ObtenerPorId(concurso.id_concurso);
                if (anterior == null)
                    throw new ValidacionException("El concurso no existe.");

                Actualizar(concurso);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "Concurso",
                    new { anterior.id_concurso, anterior.codigo, anterior.nombre, anterior.estado },
                    new { concurso.id_concurso, concurso.codigo, concurso.nombre, concurso.estado });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Concurso", ex.Message);
                throw;
            }
        }

        public void CambiarEstadoConcurso(
            int idConcurso,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(idConcurso, "El concurso indicado no es válido.");

                var concurso = ObtenerPorId(idConcurso);
                if (concurso == null)
                    throw new ValidacionException("El concurso no existe.");

                string estadoAnterior = concurso.estado;
                CambiarEstado(idConcurso, estadoAnterior);
                string estadoNuevo = estadoAnterior == "VIGENTE" ? "VENCIDO" : "VIGENTE";

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "Concurso",
                    new { concurso.id_concurso, concurso.codigo, estado = estadoAnterior },
                    new { concurso.id_concurso, concurso.codigo, estado = estadoNuevo });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Concurso", ex.Message);
                throw;
            }
        }

        public bool EliminarConcurso(
            int idConcurso,
            int? idUsuarioEjecuta,
            out string? mensajeError)
        {
            mensajeError = null;

            try
            {
                ValidarId(idConcurso, "El concurso indicado no es válido.");

                var concurso = ObtenerPorId(idConcurso);
                if (concurso == null)
                {
                    mensajeError = "El registro no existe.";
                    return false;
                }

                if (!PuedeEliminar(idConcurso))
                {
                    mensajeError = "No se puede eliminar un concurso con oferentes asociados.";
                    return false;
                }

                Eliminar(idConcurso);

                _bitacoraService.RegistrarDelete(
                    idUsuarioEjecuta,
                    "Concurso",
                    new { concurso.id_concurso, concurso.codigo, concurso.nombre });

                return true;
            }
            catch (ValidacionException ex)
            {
                mensajeError = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Concurso", ex.Message);
                throw;
            }
        }

        private static void ValidarConcurso(Concurso concurso)
        {
            if (concurso == null)
                throw new ValidacionException("Debe indicar la información del concurso.");

            concurso.codigo = NormalizarTexto(concurso.codigo);
            if (string.IsNullOrWhiteSpace(concurso.codigo))
                throw new ValidacionException("El código del concurso es obligatorio.");
            if (concurso.codigo.Length > 50)
                throw new ValidacionException("El código del concurso no puede superar los 50 caracteres.");

            concurso.nombre = NormalizarTexto(concurso.nombre);
            if (string.IsNullOrWhiteSpace(concurso.nombre))
                throw new ValidacionException("El nombre del concurso es obligatorio.");
            if (concurso.nombre.Length > 150)
                throw new ValidacionException("El nombre del concurso no puede superar los 150 caracteres.");

            if (concurso.fecha_inicio == default)
                throw new ValidacionException("La fecha de inicio del concurso es obligatoria.");

            if (concurso.fecha_fin == default)
                throw new ValidacionException("La fecha de fin del concurso es obligatoria.");

            if (concurso.fecha_fin < concurso.fecha_inicio)
                throw new ValidacionException("La fecha de fin no puede ser anterior a la fecha de inicio.");

            if (string.IsNullOrWhiteSpace(concurso.estado) || !EstadosValidos.Contains(concurso.estado))
                throw new ValidacionException("El estado del concurso debe ser VIGENTE o VENCIDO.");
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
