using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class RolesRepository
    {
        private readonly DbContext _context;

        public RolesRepository(DbContext context)
        {
            _context = context;
        }

        public List<Rol> Listar()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_rol, nombre, activo
                FROM rol
                ORDER BY id_rol DESC;";

            return connection.Query<Rol>(sql).ToList();
        }

        public Rol ObtenerPorId(int id_rol)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_rol, nombre, activo
                FROM rol
                WHERE id_rol = @id_rol;";

            return connection.QueryFirstOrDefault<Rol>(
                sql,
                new { id_rol }
            );
        }

        public List<Pantalla> ListarPantallas()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT 
                    id_pantalla,
                    nombre,
                    modulo,
                    ruta,
                    icono,
                    orden_menu,
                    visible_menu,
                    activo
                FROM pantalla
                WHERE activo = 1
                ORDER BY modulo, orden_menu, nombre;";

            return connection.Query<Pantalla>(sql).ToList();
        }

        public List<int> ObtenerPantallasDelRol(int id_rol)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_pantalla
                FROM rol_pantalla
                WHERE id_rol = @id_rol;";

            return connection.Query<int>(
                sql,
                new { id_rol }
            ).ToList();
        }

        public int Crear(Rol rol, List<int> pantallas)
        {
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlRol = @"
                    INSERT INTO rol(nombre, activo)
                    VALUES(@nombre, @activo);

                    SELECT LAST_INSERT_ID();";

                int idRol = connection.ExecuteScalar<int>(
                    sqlRol,
                    rol,
                    transaction
                );

                InsertarPantallasRol(
                    connection,
                    transaction,
                    idRol,
                    pantallas
                );

                transaction.Commit();

                return idRol;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Actualizar(Rol rol, List<int> pantallas)
        {
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlRol = @"
                    UPDATE rol
                    SET nombre = @nombre,
                        activo = @activo
                    WHERE id_rol = @id_rol;";

                connection.Execute(
                    sqlRol,
                    rol,
                    transaction
                );

                string sqlEliminarPantallas = @"
                    DELETE FROM rol_pantalla
                    WHERE id_rol = @id_rol;";

                connection.Execute(
                    sqlEliminarPantallas,
                    new { rol.id_rol },
                    transaction
                );

                InsertarPantallasRol(
                    connection,
                    transaction,
                    rol.id_rol,
                    pantallas
                );

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void InsertarPantallasRol(
            System.Data.IDbConnection connection,
            System.Data.IDbTransaction transaction,
            int idRol,
            List<int> pantallas)
        {
            if (pantallas == null || !pantallas.Any())
            {
                return;
            }

            string sql = @"
                INSERT INTO rol_pantalla(id_rol, id_pantalla)
                VALUES(@id_rol, @id_pantalla);";

            foreach (var idPantalla in pantallas)
            {
                connection.Execute(
                    sql,
                    new
                    {
                        id_rol = idRol,
                        id_pantalla = idPantalla
                    },
                    transaction
                );
            }
        }

        public bool PuedeEliminar(int id_rol)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM usuario_rol
                WHERE id_rol = @id_rol;";

            int cantidadUsuarios = connection.ExecuteScalar<int>(
                sql,
                new { id_rol }
            );

            return cantidadUsuarios == 0;
        }

        public void Eliminar(int id_rol)
        {
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlPantallas = @"
                    DELETE FROM rol_pantalla
                    WHERE id_rol = @id_rol;";

                connection.Execute(
                    sqlPantallas,
                    new { id_rol },
                    transaction
                );

                string sqlRol = @"
                    DELETE FROM rol
                    WHERE id_rol = @id_rol;";

                connection.Execute(
                    sqlRol,
                    new { id_rol },
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

        public List<Rol> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
        SELECT id_rol, nombre, activo
        FROM rol
        ORDER BY id_rol DESC
        LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<Rol>(sql, new
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
        FROM rol;";

            return connection.ExecuteScalar<int>(sql);
        }
    }
}
