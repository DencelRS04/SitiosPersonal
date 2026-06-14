using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class PuestosRepository
    {
        private readonly DbContext _context;

        public PuestosRepository(DbContext context)
        {
            _context = context;
        }

        public List<PuestoListaItem> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
                SELECT
                    p.id_puesto,
                    p.codigo,
                    p.nombre,
                    p.salario AS monto_salario,
                    j.nombre AS NombreJefatura
                FROM puesto p
                LEFT JOIN puesto j ON j.id_puesto = p.id_puesto_jefe
                ORDER BY p.id_puesto DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<PuestoListaItem>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM puesto;");
        }

        public Puesto ObtenerPorId(int id_puesto)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_puesto, codigo, nombre, salario AS monto_salario, id_puesto_jefe AS id_puesto_jefatura
                FROM puesto
                WHERE id_puesto = @id_puesto;";

            return connection.QueryFirstOrDefault<Puesto>(sql, new { id_puesto });
        }

        public List<Puesto> ListarTodos()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_puesto, codigo, nombre, salario AS monto_salario, id_puesto_jefe AS id_puesto_jefatura
                FROM puesto
                ORDER BY nombre;";

            return connection.Query<Puesto>(sql).ToList();
        }

        public bool ExisteCodigo(string codigo, int? idExcluir = null)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM puesto
                WHERE codigo = @codigo
                  AND (@idExcluir IS NULL OR id_puesto <> @idExcluir);";

            return connection.ExecuteScalar<int>(sql, new { codigo, idExcluir }) > 0;
        }

        public int Crear(Puesto puesto)
        {
            using var connection = _context.CreateConnection();

            int? idArea = connection.ExecuteScalar<int?>("SELECT id_area FROM area ORDER BY id_area LIMIT 1;");
            if (!idArea.HasValue)
                throw new InvalidOperationException("Debe existir al menos un area registrada antes de crear puestos.");

            string sql = @"
                INSERT INTO puesto(codigo, nombre, salario, id_puesto_jefe, id_area)
                VALUES(@codigo, @nombre, @monto_salario, @id_puesto_jefatura, @id_area);
                SELECT LAST_INSERT_ID();";

            return connection.ExecuteScalar<int>(sql, new
            {
                puesto.codigo,
                puesto.nombre,
                puesto.monto_salario,
                puesto.id_puesto_jefatura,
                id_area = idArea.Value
            });
        }

        public void Actualizar(Puesto puesto)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE puesto
                SET codigo             = @codigo,
                    nombre             = @nombre,
                    salario            = @monto_salario,
                    id_puesto_jefe     = @id_puesto_jefatura
                WHERE id_puesto = @id_puesto;";

            connection.Execute(sql, puesto);
        }

        public bool PuedeEliminar(int id_puesto)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*) FROM empleado WHERE id_puesto = @id_puesto
                UNION ALL
                SELECT COUNT(*) FROM puesto WHERE id_puesto_jefe = @id_puesto
                UNION ALL
                SELECT COUNT(*) FROM requisito_puesto WHERE id_puesto = @id_puesto;";

            var counts = connection.Query<int>(sql, new { id_puesto }).ToList();
            return counts.All(c => c == 0);
        }

        public void Eliminar(int id_puesto)
        {
            using var connection = _context.CreateConnection();
            connection.Execute("DELETE FROM puesto WHERE id_puesto = @id_puesto;", new { id_puesto });
        }
    }
}