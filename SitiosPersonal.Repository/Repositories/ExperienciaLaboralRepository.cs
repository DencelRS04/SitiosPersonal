using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class ExperienciaLaboralRepository
    {
        private readonly DbContext _context;

        public ExperienciaLaboralRepository(DbContext context)
        {
            _context = context;
        }

        public List<ExperienciaLaboralItem> ListarPorOferente(int id_oferente)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT
                    id_experiencia,
                    id_oferente,
                    empresa,
                    puesto,
                    fecha_inicio,
                    fecha_fin
                FROM experiencia_laboral
                WHERE id_oferente = @id_oferente
                ORDER BY fecha_inicio DESC;";

            return connection.Query<ExperienciaLaboralItem>(sql, new { id_oferente }).ToList();
        }

        public ExperienciaLaboral ObtenerPorId(int id_experiencia)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_experiencia, id_oferente, empresa, puesto, fecha_inicio, fecha_fin
                FROM experiencia_laboral
                WHERE id_experiencia = @id_experiencia;";

            return connection.QueryFirstOrDefault<ExperienciaLaboral>(sql, new { id_experiencia });
        }

        public int Crear(ExperienciaLaboral experiencia)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO experiencia_laboral(id_oferente, empresa, puesto, fecha_inicio, fecha_fin)
                VALUES(@id_oferente, @empresa, @puesto, @fecha_inicio, @fecha_fin);
                SELECT LAST_INSERT_ID();";

            return connection.ExecuteScalar<int>(sql, experiencia);
        }

        public void Actualizar(ExperienciaLaboral experiencia)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE experiencia_laboral
                SET empresa      = @empresa,
                    puesto       = @puesto,
                    fecha_inicio = @fecha_inicio,
                    fecha_fin    = @fecha_fin
                WHERE id_experiencia = @id_experiencia;";

            connection.Execute(sql, experiencia);
        }

        public bool PuedeEliminar(int id_experiencia)
        {
            // No hay tablas hijas que referencien experiencia_laboral
            return true;
        }

        public void Eliminar(int id_experiencia)
        {
            using var connection = _context.CreateConnection();

            string sql = "DELETE FROM experiencia_laboral WHERE id_experiencia = @id_experiencia;";

            connection.Execute(sql, new { id_experiencia });
        }
    }
}
