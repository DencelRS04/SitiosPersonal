using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class EntrevistasRepository
    {
        private readonly DbContext _context;

        public EntrevistasRepository(DbContext context)
        {
            _context = context;
        }

        public List<EntrevistaListaItem> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
                SELECT
                    e.id_entrevista,
                    e.id_oferente,
                    ofe.nombre_completo  AS NombreOferente,
                    emp_ofe.nombre_completo AS NombreEmpleado,
                    emp.numero_empleado  AS NumeroEmpleado,
                    e.fecha_entrevista,
                    e.estado
                FROM entrevista e
                INNER JOIN oferente ofe       ON ofe.id_oferente = e.id_oferente
                INNER JOIN empleado emp        ON emp.id_empleado  = e.id_empleado_entrevistador
                INNER JOIN oferente emp_ofe    ON emp_ofe.id_oferente = emp.id_oferente
                ORDER BY e.fecha_entrevista ASC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<EntrevistaListaItem>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();

            string sql = "SELECT COUNT(*) FROM entrevista;";

            return connection.ExecuteScalar<int>(sql);
        }

        public Entrevista ObtenerPorId(int id_entrevista)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_entrevista, id_oferente, id_empleado_entrevistador,
                       fecha_entrevista, estado, observacion, fecha_creacion
                FROM entrevista
                WHERE id_entrevista = @id_entrevista;";

            return connection.QueryFirstOrDefault<Entrevista>(sql, new { id_entrevista });
        }

        public List<OferenteDropdownItem> ListarOferentes()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_oferente, nombre_completo
                FROM oferente
                ORDER BY nombre_completo;";

            return connection.Query<OferenteDropdownItem>(sql).ToList();
        }

        public List<EmpleadoDropdownItem> ListarEmpleados()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT emp.id_empleado, emp.numero_empleado, o.nombre_completo
                FROM empleado emp
                INNER JOIN oferente o ON o.id_oferente = emp.id_oferente
                ORDER BY o.nombre_completo;";

            return connection.Query<EmpleadoDropdownItem>(sql).ToList();
        }

        public int Crear(Entrevista entrevista)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO entrevista(id_oferente, id_empleado_entrevistador, fecha_entrevista, estado)
                VALUES(@id_oferente, @id_empleado_entrevistador, @fecha_entrevista, 'PENDIENTE');
                SELECT LAST_INSERT_ID();";

            return connection.ExecuteScalar<int>(sql, entrevista);
        }

        public void Actualizar(Entrevista entrevista)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE entrevista
                SET id_empleado_entrevistador = @id_empleado_entrevistador,
                    fecha_entrevista          = @fecha_entrevista
                WHERE id_entrevista = @id_entrevista;";

            connection.Execute(sql, entrevista);
        }

        public void MarcarRealizada(int id_entrevista)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE entrevista
                SET estado = 'REALIZADA'
                WHERE id_entrevista = @id_entrevista;";

            connection.Execute(sql, new { id_entrevista });
        }

        public bool PuedeEliminar(int id_entrevista)
        {
            // No hay tablas hijas que referencien entrevista
            return true;
        }

        public void Eliminar(int id_entrevista)
        {
            using var connection = _context.CreateConnection();

            string sql = "DELETE FROM entrevista WHERE id_entrevista = @id_entrevista;";

            connection.Execute(sql, new { id_entrevista });
        }
    }
}
