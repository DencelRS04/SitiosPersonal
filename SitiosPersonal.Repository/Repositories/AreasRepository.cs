using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class AreasRepository
    {
        private readonly DbContext _context;

        public AreasRepository(DbContext context)
        {
            _context = context;
        }

        public List<AreaListaItem> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
                SELECT
                    a.id_area,
                    a.codigo,
                    a.nombre,
                    o.nombre_completo AS NombreJefatura
                FROM area a
                LEFT JOIN empleado emp ON emp.id_empleado = a.id_jefatura
                LEFT JOIN oferente o ON o.id_oferente = emp.id_oferente
                ORDER BY a.id_area DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<AreaListaItem>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM area;");
        }

        public Area ObtenerPorId(int id_area)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_area, codigo, nombre, id_jefatura AS id_empleado_jefatura
                FROM area
                WHERE id_area = @id_area;";

            return connection.QueryFirstOrDefault<Area>(sql, new { id_area });
        }

        public bool ExisteCodigo(string codigo, int? idExcluir = null)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM area
                WHERE codigo = @codigo
                  AND (@idExcluir IS NULL OR id_area <> @idExcluir);";

            return connection.ExecuteScalar<int>(sql, new { codigo, idExcluir }) > 0;
        }

        public int Crear(Area area)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO area(codigo, nombre, id_jefatura)
                VALUES(@codigo, @nombre, @id_empleado_jefatura);
                SELECT LAST_INSERT_ID();";

            return connection.ExecuteScalar<int>(sql, area);
        }

        public void Actualizar(Area area)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE area
                SET codigo   = @codigo,
                    nombre   = @nombre,
                    id_jefatura = @id_empleado_jefatura
                WHERE id_area = @id_area;";

            connection.Execute(sql, area);
        }

        public bool PuedeEliminar(int id_area)
        {
            // El área no tiene tablas hijas en el esquema actual
            return true;
        }

        public void Eliminar(int id_area)
        {
            using var connection = _context.CreateConnection();
            connection.Execute("DELETE FROM area WHERE id_area = @id_area;", new { id_area });
        }
    }
}
