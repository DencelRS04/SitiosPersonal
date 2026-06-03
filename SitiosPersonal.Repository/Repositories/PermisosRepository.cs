using Dapper;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class PermisosRepository
    {
        private readonly DbContext _context;

        public PermisosRepository(DbContext context)
        {
            _context = context;
        }

        public List<string> ObtenerRutasPermitidas(int id_usuario)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT DISTINCT p.ruta
                FROM pantalla p
                INNER JOIN rol_pantalla rp 
                    ON p.id_pantalla = rp.id_pantalla
                INNER JOIN usuario_rol ur 
                    ON rp.id_rol = ur.id_rol
                WHERE ur.id_usuario = @id_usuario
                  AND p.activo = 1
                  AND p.ruta IS NOT NULL
                  AND p.ruta <> '#';";

            return connection.Query<string>(
                sql,
                new { id_usuario }
            ).ToList();
        }
    }
}