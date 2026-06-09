using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class AccionesPersonalRepository
    {
        private readonly DbContext _context;

        public AccionesPersonalRepository(DbContext context)
        {
            _context = context;
        }

        public List<AccionPersonalListaItem> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
                SELECT
                    ap.id_accion,
                    CONCAT('AP-', LPAD(ap.id_accion, 6, '0')) AS codigo,
                    ap.fecha_accion,
                    ap.descripcion,
                    ofe.nombre_completo  AS NombreEmpleado,
                    jef.nombre_completo  AS NombreJefatura
                FROM accion_personal ap
                INNER JOIN empleado emp  ON emp.id_empleado = ap.id_empleado
                INNER JOIN oferente ofe  ON ofe.id_oferente  = emp.id_oferente
                INNER JOIN empleado ejef ON ejef.id_empleado = ap.id_aprobador
                INNER JOIN oferente jef  ON jef.id_oferente  = ejef.id_oferente
                ORDER BY ap.id_accion DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<AccionPersonalListaItem>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM accion_personal;");
        }

        public AccionPersonal? ObtenerPorId(int id_accion)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_accion,
                       CONCAT('AP-', LPAD(id_accion, 6, '0')) AS codigo,
                       fecha_accion,
                       descripcion,
                       id_empleado,
                       id_aprobador AS id_empleado_jefatura
                FROM accion_personal
                WHERE id_accion = @id_accion;";

            return connection.QueryFirstOrDefault<AccionPersonal>(sql, new { id_accion });
        }

        public string GenerarCodigo()
        {
            using var connection = _context.CreateConnection();
            int siguiente = connection.ExecuteScalar<int>("SELECT COALESCE(MAX(id_accion), 0) + 1 FROM accion_personal;");
            return $"AP-{siguiente:D6}";
        }

        public int Crear(AccionPersonal accion)
        {
            using var connection = _context.CreateConnection();

            // Generar el siguiente ID disponible
            int nuevoId = connection.ExecuteScalar<int>("SELECT COALESCE(MAX(id_accion), 0) + 1 FROM accion_personal;");
            
            accion.id_accion = nuevoId;

            string sql = @"
                INSERT INTO accion_personal(id_accion, fecha_accion, descripcion, id_empleado, id_aprobador)
                VALUES(@id_accion, @fecha_accion, @descripcion, @id_empleado, @id_empleado_jefatura);";

            connection.Execute(sql, accion);
            return nuevoId;
        }

        public void Actualizar(AccionPersonal accion)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE accion_personal
                SET fecha_accion          = @fecha_accion,
                    descripcion           = @descripcion,
                    id_empleado           = @id_empleado,
                    id_aprobador          = @id_empleado_jefatura
                WHERE id_accion = @id_accion;";

            connection.Execute(sql, accion);
        }

        public bool PuedeEliminar(int id_accion)
        {
            // Las acciones de personal no tienen tablas hijas
            return true;
        }

        public void Eliminar(int id_accion)
        {
            using var connection = _context.CreateConnection();
            connection.Execute("DELETE FROM accion_personal WHERE id_accion = @id_accion;", new { id_accion });
        }
    }
}
