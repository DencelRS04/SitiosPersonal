using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class EmpleadosRepository
    {
        private readonly DbContext _context;

        public EmpleadosRepository(DbContext context)
        {
            _context = context;
        }

        public List<EmpleadoListaItem> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
                SELECT
                    emp.id_empleado,
                    emp.numero_empleado,
                    o.nombre_completo,
                    o.identificacion,
                    p.nombre AS NombrePuesto,
                    emp.fecha_ingreso AS fecha_contratacion,
                    'ACTIVO' AS estado
                FROM empleado emp
                INNER JOIN oferente o ON o.id_oferente = emp.id_oferente
                INNER JOIN puesto p ON p.id_puesto = emp.id_puesto
                ORDER BY emp.id_empleado DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<EmpleadoListaItem>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM empleado;");
        }

        public Empleado ObtenerPorId(int id_empleado)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_empleado, numero_empleado, id_oferente, id_puesto, fecha_ingreso AS fecha_contratacion, 'ACTIVO' AS estado
                FROM empleado
                WHERE id_empleado = @id_empleado;";

            return connection.QueryFirstOrDefault<Empleado>(sql, new { id_empleado });
        }

        public bool OferenteYaEsEmpleado(int id_oferente)
        {
            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM empleado WHERE id_oferente = @id_oferente;",
                new { id_oferente }) > 0;
        }

        public string GenerarNumeroEmpleado()
        {
            using var connection = _context.CreateConnection();
            int siguiente = connection.ExecuteScalar<int>("SELECT COALESCE(MAX(id_empleado), 0) + 1 FROM empleado;");
            return $"EMP-{siguiente:D6}";
        }

        public List<OferenteDropdownItem> ListarOferentesDisponibles()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT o.id_oferente, o.nombre_completo
                FROM oferente o
                WHERE o.id_oferente NOT IN (SELECT id_oferente FROM empleado)
                ORDER BY o.nombre_completo;";

            return connection.Query<OferenteDropdownItem>(sql).ToList();
        }

        public List<PuestoDropdownItem> ListarPuestos()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_puesto, codigo, nombre
                FROM puesto
                ORDER BY nombre;";

            return connection.Query<PuestoDropdownItem>(sql).ToList();
        }

        public List<EmpleadoDropdownItem> ListarTodosDropdown()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT emp.id_empleado, emp.numero_empleado, o.nombre_completo
                FROM empleado emp
                INNER JOIN oferente o ON o.id_oferente = emp.id_oferente
                ORDER BY o.nombre_completo;";

            return connection.Query<EmpleadoDropdownItem>(sql).ToList();
        }

        public int Contratar(Empleado empleado)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO empleado(numero_empleado, id_oferente, id_puesto, fecha_ingreso)
                VALUES(@numero_empleado, @id_oferente, @id_puesto, @fecha_contratacion);
                SELECT LAST_INSERT_ID();";

            return connection.ExecuteScalar<int>(sql, empleado);
        }
    }
}
