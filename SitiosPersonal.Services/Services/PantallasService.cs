using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Exceptions;
using System.Text.RegularExpressions;

namespace SitiosPersonal.Services.Services
{
    public class PantallasService : PantallasRepository
    {
        private readonly BitacoraService _bitacoraService;

        private static readonly Regex SoloLetrasEspaciosRegex = new(
            @"^[a-zA-Z·ÈÌÛ˙¡…Õ”⁄Ò—\s]+$",
            RegexOptions.Compiled);

        public PantallasService(
            DbContext context,
            BitacoraService bitacoraService) : base(context)
        {
            _bitacoraService = bitacoraService;
        }

        public PantallasListaViewModel ObtenerListado(
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
                var lista = new PantallasListaViewModel
                {
                    Pagina = pagina,
                    CantidadPorPagina = cantidadPorPagina,
                    TotalRegistros = Contar(),
                    Pantallas = ListarPaginado(pagina, cantidadPorPagina)
                };

                _bitacoraService.RegistrarConsulta(idUsuarioEjecuta, "Pantallas");

                return lista;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Pantalla", ex.Message);
                throw;
            }
        }

        public int CrearPantalla(
            Pantalla pantalla,
            List<int> rolesSeleccionados,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarPantalla(pantalla, rolesSeleccionados);

                int idPantalla = Crear(pantalla, rolesSeleccionados);
                pantalla.id_pantalla = idPantalla;

                _bitacoraService.RegistrarInsert(
                    idUsuarioEjecuta,
                    "Pantalla",
                    new
                    {
                        pantalla.id_pantalla,
                        pantalla.nombre,
                        pantalla.modulo,
                        pantalla.ruta,
                        pantalla.icono,
                        pantalla.orden_menu,
                        pantalla.visible_menu,
                        pantalla.activo,
                        roles = rolesSeleccionados
                    });

                return idPantalla;
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Pantalla", ex.Message);
                throw;
            }
        }

        public void ActualizarPantalla(
            Pantalla pantallaActual,
            List<int> rolesSeleccionados,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(pantallaActual.id_pantalla, "La pantalla indicada no es v·lida.");
                ValidarPantalla(pantallaActual, rolesSeleccionados);

                var pantallaAnterior = ObtenerPorId(pantallaActual.id_pantalla);
                var rolesAnteriores = ObtenerRolesDePantalla(pantallaActual.id_pantalla);

                if (pantallaAnterior == null)
                {
                    throw new ValidacionException("La pantalla no existe.");
                }

                Actualizar(pantallaActual, rolesSeleccionados);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "Pantalla",
                    new
                    {
                        pantallaAnterior.id_pantalla,
                        pantallaAnterior.nombre,
                        pantallaAnterior.modulo,
                        pantallaAnterior.ruta,
                        pantallaAnterior.icono,
                        pantallaAnterior.orden_menu,
                        pantallaAnterior.visible_menu,
                        pantallaAnterior.activo,
                        roles = rolesAnteriores
                    },
                    new
                    {
                        pantallaActual.id_pantalla,
                        pantallaActual.nombre,
                        roles = rolesSeleccionados
                    });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Pantalla", ex.Message);
                throw;
            }
        }

        public bool EliminarPantalla(
            int idPantalla,
            int? idUsuarioEjecuta,
            out string? mensajeError)
        {
            mensajeError = null;

            try
            {
                ValidarId(idPantalla, "La pantalla indicada no es v·lida.");

                var pantalla = ObtenerPorId(idPantalla);

                if (pantalla == null)
                {
                    mensajeError = "El registro no existe.";
                    return false;
                }

                if (!PuedeEliminar(idPantalla))
                {
                    mensajeError = "No se puede eliminar un registro con datos relacionados.";
                    return false;
                }

                Eliminar(idPantalla);

                _bitacoraService.RegistrarDelete(
                    idUsuarioEjecuta,
                    "Pantalla",
                    new
                    {
                        pantalla.id_pantalla,
                        pantalla.nombre,
                        pantalla.modulo,
                        pantalla.ruta,
                        pantalla.icono,
                        pantalla.orden_menu,
                        pantalla.visible_menu,
                        pantalla.activo
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
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Pantalla", ex.Message);
                throw;
            }
        }

        private static void ValidarPantalla(Pantalla pantalla, List<int> rolesSeleccionados)
        {
            if (pantalla == null)
            {
                throw new ValidacionException("Debe indicar la informaciÛn de la pantalla.");
            }

            pantalla.nombre = NormalizarTexto(pantalla.nombre);

            if (string.IsNullOrWhiteSpace(pantalla.nombre))
            {
                throw new ValidacionException("El nombre de la pantalla es obligatorio.");
            }

            if (pantalla.nombre.Length > 100)
            {
                throw new ValidacionException("El nombre de la pantalla no puede superar los 100 caracteres.");
            }

            if (!SoloLetrasEspaciosRegex.IsMatch(pantalla.nombre))
            {
                throw new ValidacionException("El nombre de la pantalla solo puede contener letras y espacios.");
            }

            if (rolesSeleccionados == null || !rolesSeleccionados.Any())
            {
                throw new ValidacionException("Debe seleccionar al menos un rol para la pantalla.");
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