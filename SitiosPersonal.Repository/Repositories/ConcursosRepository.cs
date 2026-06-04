using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class ConcursosRepository
    {
        private readonly DbContext _context;

        public ConcursosRepository(DbContext context)
        {
            _context = context;
        }

        public List<Concurso> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
                SELECT id_concurso, codigo, nombre, fecha_inicio, fecha_fin, estado
                FROM concurso
                ORDER BY id_concurso DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<Concurso>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();

            string sql = "SELECT COUNT(*) FROM concurso;";

            return connection.ExecuteScalar<int>(sql);
        }

        public Concurso ObtenerPorId(int id_concurso)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_concurso, codigo, nombre, fecha_inicio, fecha_fin, estado
                FROM concurso
                WHERE id_concurso = @id_concurso;";

            return connection.QueryFirstOrDefault<Concurso>(sql, new { id_concurso });
        }

        public bool ExisteCodigo(string codigo, int? idExcluir = null)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM concurso
                WHERE codigo = @codigo
                  AND (@idExcluir IS NULL OR id_concurso <> @idExcluir);";

            return connection.ExecuteScalar<int>(sql, new { codigo, idExcluir }) > 0;
        }

        public int Crear(Concurso concurso)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO concurso(codigo, nombre, fecha_inicio, fecha_fin, estado)
                VALUES(@codigo, @nombre, @fecha_inicio, @fecha_fin, @estado);
                SELECT LAST_INSERT_ID();";

            return connection.ExecuteScalar<int>(sql, concurso);
        }

        public void Actualizar(Concurso concurso)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE concurso
                SET codigo       = @codigo,
                    nombre       = @nombre,
                    fecha_inicio = @fecha_inicio,
                    fecha_fin    = @fecha_fin,
                    estado       = @estado
                WHERE id_concurso = @id_concurso;";

            connection.Execute(sql, concurso);
        }

        public void CambiarEstado(int id_concurso, string estadoActual)
        {
            using var connection = _context.CreateConnection();

            string nuevoEstado = estadoActual == "VIGENTE" ? "VENCIDO" : "VIGENTE";

            string sql = @"
                UPDATE concurso
                SET estado = @nuevoEstado
                WHERE id_concurso = @id_concurso;";

            connection.Execute(sql, new { nuevoEstado, id_concurso });
        }

        public bool PuedeEliminar(int id_concurso)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM oferente_concurso
                WHERE id_concurso = @id_concurso;";

            return connection.ExecuteScalar<int>(sql, new { id_concurso }) == 0;
        }

        public void Eliminar(int id_concurso)
        {
            using var connection = _context.CreateConnection();

            string sql = "DELETE FROM concurso WHERE id_concurso = @id_concurso;";

            connection.Execute(sql, new { id_concurso });
        }
    }
}
