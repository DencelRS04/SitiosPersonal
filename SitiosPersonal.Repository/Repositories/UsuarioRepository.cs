using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class UsuariosRepository
    {
        private readonly DbContext _context;

        public UsuariosRepository(DbContext context)
        {
            _context = context;
        }

        public List<Usuario> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
                SELECT
                    id_usuario,
                    usuario,
                    nombre_completo,
                    correo,
                    password_hash,
                    estado,
                    intentos_login
                FROM usuario
                ORDER BY id_usuario DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<Usuario>(sql, new
            {
                cantidadPorPagina,
                offset
            }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM usuario;";

            return connection.ExecuteScalar<int>(sql);
        }

        public Usuario ObtenerPorId(int id_usuario)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT
                    id_usuario,
                    usuario,
                    nombre_completo,
                    correo,
                    password_hash,
                    estado,
                    intentos_login
                FROM usuario
                WHERE id_usuario = @id_usuario;";

            return connection.QueryFirstOrDefault<Usuario>(
                sql,
                new { id_usuario }
            );
        }

        public List<Rol> ListarRoles()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_rol, nombre, activo
                FROM rol
                ORDER BY nombre ASC;";

            return connection.Query<Rol>(sql).ToList();
        }

        public List<int> ObtenerRolesDelUsuario(int id_usuario)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_rol
                FROM usuario_rol
                WHERE id_usuario = @id_usuario;";

            return connection.Query<int>(
                sql,
                new { id_usuario }
            ).ToList();
        }

        public int Crear(Usuario usuario, List<int> roles)
        {
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlUsuario = @"
                    INSERT INTO usuario
                    (
                        usuario,
                        nombre_completo,
                        correo,
                        password_hash,
                        estado,
                        intentos_login
                    )
                    VALUES
                    (
                        @usuario,
                        @nombre_completo,
                        @correo,
                        @password_hash,
                        @estado,
                        0
                    );

                    SELECT LAST_INSERT_ID();";

                int idUsuario = connection.ExecuteScalar<int>(
                    sqlUsuario,
                    usuario,
                    transaction
                );

                InsertarRolesUsuario(
                    connection,
                    transaction,
                    idUsuario,
                    roles
                );

                transaction.Commit();

                return idUsuario;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Actualizar(Usuario usuario, List<int> roles)
        {
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlUsuario = @"
                    UPDATE usuario
                    SET usuario = @usuario,
                        nombre_completo = @nombre_completo,
                        correo = @correo,
                        password_hash = @password_hash,
                        estado = @estado
                    WHERE id_usuario = @id_usuario;";

                connection.Execute(
                    sqlUsuario,
                    usuario,
                    transaction
                );

                string sqlEliminarRoles = @"
                    DELETE FROM usuario_rol
                    WHERE id_usuario = @id_usuario;";

                connection.Execute(
                    sqlEliminarRoles,
                    new { usuario.id_usuario },
                    transaction
                );

                InsertarRolesUsuario(
                    connection,
                    transaction,
                    usuario.id_usuario,
                    roles
                );

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void InsertarRolesUsuario(
            System.Data.IDbConnection connection,
            System.Data.IDbTransaction transaction,
            int idUsuario,
            List<int> roles)
        {
            if (roles == null || !roles.Any())
            {
                return;
            }

            string sql = @"
                INSERT INTO usuario_rol(id_usuario, id_rol)
                VALUES(@id_usuario, @id_rol);";

            foreach (var idRol in roles)
            {
                connection.Execute(
                    sql,
                    new
                    {
                        id_usuario = idUsuario,
                        id_rol = idRol
                    },
                    transaction
                );
            }
        }

        public bool PuedeEliminar(int id_usuario)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM bitacora
                WHERE id_usuario = @id_usuario;";

            int cantidad = connection.ExecuteScalar<int>(
                sql,
                new { id_usuario }
            );

            return cantidad == 0;
        }

        public void Eliminar(int id_usuario)
        {
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlRoles = @"
                    DELETE FROM usuario_rol
                    WHERE id_usuario = @id_usuario;";

                connection.Execute(
                    sqlRoles,
                    new { id_usuario },
                    transaction
                );

                string sqlUsuario = @"
                    DELETE FROM usuario
                    WHERE id_usuario = @id_usuario;";

                connection.Execute(
                    sqlUsuario,
                    new { id_usuario },
                    transaction
                );

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void CambiarEstado(int id_usuario, string estado)
        {
            using var connection = _context.CreateConnection();

            string sql = @"             
                UPDATE usuario
                SET estado = @estado
                WHERE id_usuario = @id_usuario;";

            connection.Execute(sql, new
            {
                id_usuario,
                estado
            });
        }
    }
}
