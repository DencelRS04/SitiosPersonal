using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class PreparacionAcademicaRepository
    {
        private readonly DbContext _context;

        public PreparacionAcademicaRepository(DbContext context)
        {
            _context = context;
        }

        public List<PreparacionAcademicaItem> ListarPorOferente(int id_oferente)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT
                    p.id_preparacion,
                    p.id_oferente,
                    i.nombre AS NombreInstitucion,
                    p.titulo,
                    p.fecha_inicio,
                    p.fecha_fin
                FROM preparacion_academica p
                INNER JOIN institucion_educativa i ON i.id_institucion = p.id_institucion
                WHERE p.id_oferente = @id_oferente
                ORDER BY p.fecha_inicio DESC;";

            return connection.Query<PreparacionAcademicaItem>(sql, new { id_oferente }).ToList();
        }

        public PreparacionAcademica ObtenerPorId(int id_preparacion)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_preparacion, id_oferente, id_institucion, titulo, fecha_inicio, fecha_fin
                FROM preparacion_academica
                WHERE id_preparacion = @id_preparacion;";

            return connection.QueryFirstOrDefault<PreparacionAcademica>(sql, new { id_preparacion });
        }

        public List<InstitucionEducativa> ListarInstituciones()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_institucion, codigo, nombre
                FROM institucion_educativa
                ORDER BY nombre;";

            return connection.Query<InstitucionEducativa>(sql).ToList();
        }

        public int Crear(PreparacionAcademica preparacion)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO preparacion_academica(id_oferente, id_institucion, titulo, fecha_inicio, fecha_fin)
                VALUES(@id_oferente, @id_institucion, @titulo, @fecha_inicio, @fecha_fin);
                SELECT LAST_INSERT_ID();";

            return connection.ExecuteScalar<int>(sql, preparacion);
        }

        public void Actualizar(PreparacionAcademica preparacion)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE preparacion_academica
                SET id_institucion = @id_institucion,
                    titulo         = @titulo,
                    fecha_inicio   = @fecha_inicio,
                    fecha_fin      = @fecha_fin
                WHERE id_preparacion = @id_preparacion;";

            connection.Execute(sql, preparacion);
        }

        public bool PuedeEliminar(int id_preparacion)
        {
            // No hay tablas hijas que referencien preparacion_academica
            return true;
        }

        public void Eliminar(int id_preparacion)
        {
            using var connection = _context.CreateConnection();

            string sql = "DELETE FROM preparacion_academica WHERE id_preparacion = @id_preparacion;";

            connection.Execute(sql, new { id_preparacion });
        }
    }
}
