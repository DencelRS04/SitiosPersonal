using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Exceptions;
using System.Text.RegularExpressions;

namespace SitiosPersonal.Services.Services
{
    public class RolesService : RolesRepository
    {
        private readonly BitacoraService _bitacoraService;

        private static readonly Regex SoloLetrasEspaciosRegex = new(
            @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$",
            RegexOptions.Compiled);

        public RolesService(
            DbContext context,
            BitacoraService bitacoraService) : base(context)
        {
            _bitacoraService = bitacoraService;
        }

        public RolesListaViewModel ObtenerListado(
            int pagina,
            int cantidadPorPagina,
            int? idUsuarioEjecuta)
        {
            if (pagina < 1)
            {
                pagina = 1;
            }

            if (cantidadPorPagina < 1)
            {
                cantidadPorPagina = 10;
            }

            try
            {
                var lista = new RolesListaViewModel
                {
                    Pagina = pagina,
                    CantidadPorPagina = cantidadPorPagina,
                    TotalRegistros = Contar(),
                    Roles = ListarPaginado(pagina, cantidadPorPagina)
                };

                _bitacoraService.RegistrarConsulta(idUsuarioEjecuta, "Roles");

                return lista;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Rol", ex.Message);
                throw;
            }
        }

        public int CrearRol(
            Rol rol,
            List<int> pantallasSeleccionadas,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarRol(rol, pantallasSeleccionadas);

                int idRol = Crear(rol, pantallasSeleccionadas);
                rol.id_rol = idRol;

                _bitacoraService.RegistrarInsert(
                    idUsuarioEjecuta,
                    "Rol",
                    new
                    {
                        rol.id_rol,
                        rol.nombre,
                        pantallas = pantallasSeleccionadas
                    });

                return idRol;
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Rol", ex.Message);
                throw;
            }
        }

        public void ActualizarRol(
            Rol rolActual,
            List<int> pantallasSeleccionadas,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(rolActual.id_rol, "El rol indicado no es válido.");
                ValidarRol(rolActual, pantallasSeleccionadas);

                var rolAnterior = ObtenerPorId(rolActual.id_rol);
                var pantallasAnteriores = ObtenerPantallasDelRol(rolActual.id_rol);

                if (rolAnterior == null)
                {
                    throw new ValidacionException("El rol no existe.");
                }

                Actualizar(rolActual, pantallasSeleccionadas);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "Rol",
                    new
                    {
                        rolAnterior.id_rol,
                        rolAnterior.nombre,
                        rolAnterior.activo,
                        pantallas = pantallasAnteriores
                    },
                    new
                    {
                        rolActual.id_rol,
                        rolActual.nombre,
                        rolActual.activo,
                        pantallas = pantallasSeleccionadas
                    });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Rol", ex.Message);
                throw;
            }
        }

        public bool EliminarRol(
            int idRol,
            int? idUsuarioEjecuta,
            out string? mensajeError)
        {
            mensajeError = null;

            try
            {
                ValidarId(idRol, "El rol indicado no es válido.");

                var rol = ObtenerPorId(idRol);

                if (rol == null)
                {
                    mensajeError = "El registro no existe.";
                    return false;
                }

                if (!PuedeEliminar(idRol))
                {
                    mensajeError = "No se puede eliminar un registro con datos relacionados.";
                    return false;
                }

                Eliminar(idRol);

                _bitacoraService.RegistrarDelete(
                    idUsuarioEjecuta,
                    "Rol",
                    new
                    {
                        rol.id_rol,
                        rol.nombre,
                        rol.activo
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
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Rol", ex.Message);
                throw;
            }
        }

        private static void ValidarRol(Rol rol, List<int> pantallasSeleccionadas)
        {
            if (rol == null)
            {
                throw new ValidacionException("Debe indicar la información del rol.");
            }

            rol.nombre = NormalizarTexto(rol.nombre);

            if (string.IsNullOrWhiteSpace(rol.nombre))
            {
                throw new ValidacionException("El nombre del rol es obligatorio.");
            }

            if (rol.nombre.Length > 40)
            {
                throw new ValidacionException("El nombre del rol no puede superar los 40 caracteres.");
            }

            if (!SoloLetrasEspaciosRegex.IsMatch(rol.nombre))
            {
                throw new ValidacionException("El nombre del rol solo puede contener letras y espacios.");
            }

            if (pantallasSeleccionadas == null || !pantallasSeleccionadas.Any())
            {
                throw new ValidacionException("Debe seleccionar al menos una pantalla para el rol.");
            }
        }

        private static void ValidarId(int id, string mensaje)
        {
            if (id <= 0)
            {
                throw new ValidacionException(mensaje);
            }
        }

        private static string NormalizarTexto(string? texto)
        {
            return string.IsNullOrWhiteSpace(texto)
                ? string.Empty
                : Regex.Replace(texto.Trim(), @"\s+", " ");
        }
    }
}