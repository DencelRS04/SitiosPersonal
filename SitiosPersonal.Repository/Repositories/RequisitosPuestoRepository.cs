using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class RequisitosPuestoRepository
    {
        private readonly DbContext _context;

        public RequisitosPuestoRepository(DbContext context)
        {
            _context = context;
        }

        public List<RequisitoPuestoListaItem> ListarPorPuesto(int id_puesto, int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
                SELECT id_requisito, id_puesto, nombre
                FROM requisito_puesto
                WHERE id_puesto = @id_puesto
                ORDER BY id_requisito DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<RequisitoPuestoListaItem>(sql, new { id_puesto, cantidadPorPagina, offset }).ToList();
        }

        public int ContarPorPuesto(int id_puesto)
        {
            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM requisito_puesto WHERE id_puesto = @id_puesto;",
                new { id_puesto });
        }

        public RequisitoPuesto ObtenerPorId(int id_requisito)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_requisito, id_puesto, nombre
                FROM requisito_puesto
                WHERE id_requisito = @id_requisito;";

            return connection.QueryFirstOrDefault<RequisitoPuesto>(sql, new { id_requisito });
        }

        public int Crear(RequisitoPuesto requisito)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO requisito_puesto(id_puesto, nombre)
                VALUES(@id_puesto, @nombre);
                SELECT LAST_INSERT_ID();";

            return connection.ExecuteScalar<int>(sql, requisito);
        }

        public void Actualizar(RequisitoPuesto requisito)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE requisito_puesto
                SET nombre = @nombre
                WHERE id_requisito = @id_requisito;";

            connection.Execute(sql, requisito);
        }

        public bool PuedeEliminar(int id_requisito)
        {
            using var connection = _context.CreateConnection();

            string relacionesSql = @"
                SELECT DISTINCT TABLE_NAME AS Tabla, COLUMN_NAME AS Columna
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND COLUMN_NAME = 'id_requisito'
                  AND TABLE_NAME <> 'requisito_puesto';";

            var relaciones = connection.Query(relacionesSql);

            foreach (var relacion in relaciones)
            {
                string tabla = relacion.Tabla;
                string columna = relacion.Columna;

                string conteoSql = $@"
                    SELECT COUNT(*)
                    FROM `{tabla}`
                    WHERE `{columna}` = @id_requisito;";

                int total = connection.ExecuteScalar<int>(conteoSql, new { id_requisito });
                if (total > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void Eliminar(int id_requisito)
        {
            using var connection = _context.CreateConnection();
            connection.Execute("DELETE FROM requisito_puesto WHERE id_requisito = @id_requisito;", new { id_requisito });
        }
    }
}
