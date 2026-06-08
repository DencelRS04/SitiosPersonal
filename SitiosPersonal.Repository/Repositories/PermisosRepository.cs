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

            if (EsAdministrador(id_usuario, connection))
            {
                string adminSql = @"
                    SELECT DISTINCT ruta
                    FROM pantalla
                    WHERE activo = 1
                      AND ruta IS NOT NULL
                      AND ruta <> '#';";

                return NormalizeRutas(connection.Query<string>(adminSql).ToList()).ToList();
            }

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

            var rutas = connection.Query<string>(
                sql,
                new { id_usuario }
            ).ToList();

            return NormalizeRutas(rutas).ToList();
        }

        public bool EsAdministrador(int id_usuario)
        {
            using var connection = _context.CreateConnection();
            return EsAdministrador(id_usuario, connection);
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

        private static IEnumerable<string> NormalizeRutas(IEnumerable<string> rutas)
        {
            return rutas
                .Where(ruta => !string.IsNullOrWhiteSpace(ruta))
                .Select(ruta => ruta.Trim())
                .Where(ruta => ruta != "#")
                .SelectMany(ruta =>
                {
                    var cleaned = ruta.TrimStart('~').TrimStart('/').TrimEnd('/');
                    if (string.IsNullOrWhiteSpace(cleaned))
                    {
                        return new[] { string.Empty };
                    }

                    var result = new List<string> { cleaned };

                    if (!cleaned.StartsWith("/", StringComparison.Ordinal))
                    {
                        result.Add($"/{cleaned}");
                    }

                    return result.Distinct(StringComparer.OrdinalIgnoreCase);
                })
                .Where(ruta => !string.IsNullOrWhiteSpace(ruta));
        }
    }
}
