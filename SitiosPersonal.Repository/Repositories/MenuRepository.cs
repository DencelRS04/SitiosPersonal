using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class MenuRepository
    {
        private readonly DbContext _context;

        public MenuRepository(DbContext context)
        {
            _context = context;
        }

        public List<Pantalla> ObtenerMenuPorUsuario(int id_usuario)
        {
            using var connection = _context.CreateConnection();

            if (EsAdministrador(id_usuario, connection))
            {
                string adminSql = @"
                    SELECT DISTINCT
                        p.id_pantalla,
                        p.nombre,
                        p.modulo,
                        p.ruta,
                        p.icono,
                        p.orden_menu,
                        p.visible_menu,
                        p.activo
                    FROM pantalla p
                    WHERE p.visible_menu = 1
                      AND p.activo = 1
                      AND p.ruta IS NOT NULL
                      AND p.ruta <> '#'
                    ORDER BY p.orden_menu ASC;";

                return connection.Query<Pantalla>(adminSql).ToList();
            }

            string sql = @"
                SELECT DISTINCT
                    p.id_pantalla,
                    p.nombre,
                    p.modulo,
                    p.ruta,
                    p.icono,
                    p.orden_menu,
                    p.visible_menu,
                    p.activo
                FROM pantalla p
                INNER JOIN rol_pantalla rp
                    ON p.id_pantalla = rp.id_pantalla
                INNER JOIN usuario_rol ur
                    ON rp.id_rol = ur.id_rol
                WHERE ur.id_usuario = @id_usuario
                  AND p.visible_menu = 1
                  AND p.activo = 1
                  AND p.ruta IS NOT NULL
                  AND p.ruta <> '#'
                ORDER BY p.orden_menu ASC;";

            return connection.Query<Pantalla>(
                sql,
                new { id_usuario }
            ).ToList();
        }

        private bool EsAdministrador(int id_usuario, System.Data.IDbConnection connection)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM usuario_rol ur
                INNER JOIN rol r ON ur.id_rol = r.id_rol
                WHERE ur.id_usuario = @id_usuario
                  AND LOWER(r.nombre) LIKE '%admin%';";

            return connection.ExecuteScalar<int>(sql, new { id_usuario }) > 0;
        }
    }
}
