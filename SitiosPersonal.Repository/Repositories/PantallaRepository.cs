using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class PantallasRepository
    {
        private readonly DbContext _context;

        public PantallasRepository(DbContext context)
        {
            _context = context;
        }

        public List<Pantalla> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

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
                ORDER BY id_pantalla DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<Pantalla>(sql, new
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
                FROM pantalla;";

            return connection.ExecuteScalar<int>(sql);
        }

        public Pantalla ObtenerPorId(int id_pantalla)
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
                WHERE id_pantalla = @id_pantalla;";

            return connection.QueryFirstOrDefault<Pantalla>(
                sql,
                new { id_pantalla }
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

        public List<int> ObtenerRolesDePantalla(int id_pantalla)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_rol
                FROM rol_pantalla
                WHERE id_pantalla = @id_pantalla;";

            return connection.Query<int>(
                sql,
                new { id_pantalla }
            ).ToList();
        }

        public int Crear(Pantalla pantalla, List<int> roles)
        {
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlPantalla = @"
                    INSERT INTO pantalla
                    (
                        nombre,
                        modulo,
                        ruta,
                        icono,
                        orden_menu,
                        visible_menu,
                        activo
                    )
                    VALUES
                    (
                        @nombre,
                        @modulo,
                        @ruta,
                        @icono,
                        @orden_menu,
                        @visible_menu,
                        @activo
                    );

                    SELECT LAST_INSERT_ID();";

                int idPantalla = connection.ExecuteScalar<int>(
                    sqlPantalla,
                    pantalla,
                    transaction
                );

                InsertarRolesPantalla(
                    connection,
                    transaction,
                    idPantalla,
                    roles
                );

                transaction.Commit();

                return idPantalla;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Actualizar(Pantalla pantalla, List<int> roles)
        {
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlPantalla = @"
                    UPDATE pantalla
                    SET nombre = @nombre
                    WHERE id_pantalla = @id_pantalla;";

                connection.Execute(
                    sqlPantalla,
                    pantalla,
                    transaction
                );

                string sqlEliminarRoles = @"
                    DELETE FROM rol_pantalla
                    WHERE id_pantalla = @id_pantalla;";

                connection.Execute(
                    sqlEliminarRoles,
                    new { pantalla.id_pantalla },
                    transaction
                );

                InsertarRolesPantalla(
                    connection,
                    transaction,
                    pantalla.id_pantalla,
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

        private void InsertarRolesPantalla(
            System.Data.IDbConnection connection,
            System.Data.IDbTransaction transaction,
            int idPantalla,
            List<int> roles)
        {
            if (roles == null || !roles.Any())
            {
                return;
            }

            string sql = @"
                INSERT INTO rol_pantalla(id_rol, id_pantalla)
                VALUES(@id_rol, @id_pantalla);";

            foreach (var idRol in roles)
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

        public bool PuedeEliminar(int id_pantalla)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM rol_pantalla
                WHERE id_pantalla = @id_pantalla;";

            int cantidad = connection.ExecuteScalar<int>(
                sql,
                new { id_pantalla }
            );

            return cantidad == 0;
        }

        public void Eliminar(int id_pantalla)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                DELETE FROM pantalla
                WHERE id_pantalla = @id_pantalla;";

            connection.Execute(sql, new { id_pantalla });
        }
    }
}
