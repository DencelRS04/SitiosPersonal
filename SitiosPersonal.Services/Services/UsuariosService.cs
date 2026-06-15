using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Exceptions;
using SitiosPersonal.Services.Helpers;
using System.Text.RegularExpressions;

namespace SitiosPersonal.Services.Services
{
    public class UsuariosService : UsuariosRepository
    {
        private readonly BitacoraService _bitacoraService;
        private readonly EncryptionHelper _encryptionHelper;

        private static readonly Regex CorreoRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled);

        private static readonly Regex PasswordSeguraRegex = new(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$",
            RegexOptions.Compiled);

        private static readonly string[] EstadosPermitidos =
        {
            "ACTIVO",
            "INACTIVO",
            "BLOQUEADO"
        };

        public UsuariosService(
            DbContext context,
            BitacoraService bitacoraService,
            EncryptionHelper encryptionHelper) : base(context)
        {
            _bitacoraService = bitacoraService;
            _encryptionHelper = encryptionHelper;
        }

        public UsuariosListaViewModel ObtenerListado(
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
                var lista = new UsuariosListaViewModel
                {
                    Pagina = pagina,
                    CantidadPorPagina = cantidadPorPagina,
                    TotalRegistros = Contar(),
                    Usuarios = ListarPaginado(pagina, cantidadPorPagina)
                };

                _bitacoraService.RegistrarConsulta(idUsuarioEjecuta, "Usuarios");

                return lista;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Usuario", ex.Message);
                throw;
            }
        }

        public int CrearUsuario(
            UsuarioViewModel model,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarUsuario(model, requierePassword: true);

                var usuario = new Usuario
                {
                    usuario = model.usuario,
                    nombre_completo = model.nombre_completo,
                    correo = model.correo,
                    password_hash = _encryptionHelper.Encriptar(model.password!),
                    estado = model.estado
                };

                int idNuevoUsuario = Crear(usuario, model.RolesSeleccionados);
                usuario.id_usuario = idNuevoUsuario;

                _bitacoraService.RegistrarInsert(
                    idUsuarioEjecuta,
                    "Usuario",
                    new
                    {
                        usuario.id_usuario,
                        usuario.usuario,
                        usuario.nombre_completo,
                        usuario.correo,
                        usuario.estado,
                        roles = model.RolesSeleccionados
                    });

                return idNuevoUsuario;
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Usuario", ex.Message);
                throw;
            }
        }

        public void ActualizarUsuario(
            int idUsuarioEditado,
            UsuarioViewModel model,
            int? idUsuarioEjecuta)
        {
            try
            {
                ValidarId(idUsuarioEditado, "El usuario indicado no es válido.");
                ValidarUsuario(model, requierePassword: false);

                var usuarioAnterior = ObtenerPorId(idUsuarioEditado);

                if (usuarioAnterior == null)
                {
                    throw new ValidacionException("El usuario no existe.");
                }

                var rolesAnteriores = ObtenerRolesDelUsuario(idUsuarioEditado);

                string passwordFinal = usuarioAnterior.password_hash;

                if (!string.IsNullOrWhiteSpace(model.password))
                {
                    passwordFinal = _encryptionHelper.Encriptar(model.password);
                }

                var usuarioActual = new Usuario
                {
                    id_usuario = idUsuarioEditado,
                    usuario = model.usuario,
                    nombre_completo = model.nombre_completo,
                    correo = model.correo,
                    password_hash = passwordFinal,
                    estado = model.estado
                };

                Actualizar(usuarioActual, model.RolesSeleccionados);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "Usuario",
                    new
                    {
                        usuarioAnterior.id_usuario,
                        usuarioAnterior.usuario,
                        usuarioAnterior.nombre_completo,
                        usuarioAnterior.correo,
                        usuarioAnterior.estado,
                        roles = rolesAnteriores
                    },
                    new
                    {
                        usuarioActual.id_usuario,
                        usuarioActual.usuario,
                        usuarioActual.nombre_completo,
                        usuarioActual.correo,
                        usuarioActual.estado,
                        roles = model.RolesSeleccionados
                    });
            }
            catch (ValidacionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Usuario", ex.Message);
                throw;
            }
        }

        public bool EliminarUsuario(
            int idUsuarioEliminado,
            int? idUsuarioEjecuta,
            out string? mensajeError)
        {
            mensajeError = null;

            try
            {
                ValidarId(idUsuarioEliminado, "El usuario indicado no es válido.");

                var usuario = ObtenerPorId(idUsuarioEliminado);

                if (usuario == null)
                {
                    mensajeError = "El registro no existe.";
                    return false;
                }

                if (!PuedeEliminar(idUsuarioEliminado))
                {
                    mensajeError = "No se puede eliminar un registro con datos relacionados.";
                    return false;
                }

                Eliminar(idUsuarioEliminado);

                _bitacoraService.RegistrarDelete(
                    idUsuarioEjecuta,
                    "Usuario",
                    new
                    {
                        usuario.id_usuario,
                        usuario.usuario,
                        usuario.nombre_completo,
                        usuario.correo,
                        usuario.estado
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
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Usuario", ex.Message);
                throw;
            }
        }

        public bool CambiarEstadoUsuario(
            int idUsuarioModificado,
            int? idUsuarioEjecuta,
            out string? mensajeError)
        {
            mensajeError = null;

            try
            {
                ValidarId(idUsuarioModificado, "El usuario indicado no es válido.");

                var usuario = ObtenerPorId(idUsuarioModificado);

                if (usuario == null)
                {
                    mensajeError = "El registro no existe.";
                    return false;
                }

                if (usuario.estado == "BLOQUEADO")
                {
                    mensajeError = "No se puede activar o inactivar un usuario bloqueado.";
                    return false;
                }

                string nuevoEstado = usuario.estado == "ACTIVO" ? "INACTIVO" : "ACTIVO";

                CambiarEstado(idUsuarioModificado, nuevoEstado);

                _bitacoraService.RegistrarUpdate(
                    idUsuarioEjecuta,
                    "Usuario",
                    new
                    {
                        usuario.id_usuario,
                        usuario.usuario,
                        usuario.estado
                    },
                    new
                    {
                        usuario.id_usuario,
                        usuario.usuario,
                        estado = nuevoEstado
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
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Usuario", ex.Message);
                throw;
            }
        }

        private static void ValidarUsuario(UsuarioViewModel model, bool requierePassword)
        {
            if (model == null)
            {
                throw new ValidacionException("Debe indicar la información del usuario.");
            }

            model.usuario = NormalizarTexto(model.usuario);
            model.nombre_completo = NormalizarTexto(model.nombre_completo);
            model.correo = NormalizarCorreo(model.correo);
            model.estado = NormalizarEstado(model.estado);

            if (string.IsNullOrWhiteSpace(model.usuario))
            {
                throw new ValidacionException("El nombre de usuario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(model.nombre_completo))
            {
                throw new ValidacionException("El nombre completo es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(model.correo))
            {
                throw new ValidacionException("El correo es obligatorio.");
            }

            if (!CorreoRegex.IsMatch(model.correo))
            {
                throw new ValidacionException("El correo no tiene un formato válido.");
            }

            if (!EstadosPermitidos.Contains(model.estado))
            {
                throw new ValidacionException("El estado debe ser ACTIVO, INACTIVO o BLOQUEADO.");
            }

            if (model.RolesSeleccionados == null || !model.RolesSeleccionados.Any())
            {
                throw new ValidacionException("Debe seleccionar al menos un rol para el usuario.");
            }

            string password = model.password ?? string.Empty;

            if (requierePassword && string.IsNullOrWhiteSpace(password))
            {
                throw new ValidacionException("La contraseña es obligatoria.");
            }

            if (!string.IsNullOrWhiteSpace(password) && !PasswordSeguraRegex.IsMatch(password))
            {
                throw new ValidacionException("La contraseña debe tener mínimo 8 caracteres, mayúsculas, minúsculas, números y caracteres especiales.");
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

        private static string NormalizarCorreo(string? correo)
        {
            return string.IsNullOrWhiteSpace(correo)
                ? string.Empty
                : correo.Trim().ToLowerInvariant();
        }

        private static string NormalizarEstado(string? estado)
        {
            return string.IsNullOrWhiteSpace(estado)
                ? string.Empty
                : estado.Trim().ToUpperInvariant();
        }
    }
}