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
                ORDER BY p.orden_menu ASC;";

            return connection.Query<Pantalla>(
                sql,
                new { id_usuario }
            ).ToList();
        }
    }
}
