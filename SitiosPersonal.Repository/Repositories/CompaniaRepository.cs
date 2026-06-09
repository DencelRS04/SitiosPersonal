using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class CompaniaRepository
    {
        private readonly DbContext _context;

        public CompaniaRepository(DbContext context)
        {
            _context = context;
        }

        public List<Compania> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();
            int offset = (pagina - 1) * cantidadPorPagina;
            string sql = @"
                SELECT id_compania, codigo, nombre
                FROM compania
                ORDER BY id_compania DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";
            return connection.Query<Compania>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM compania;");
        }

        public Compania ObtenerPorId(int id_compania)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT id_compania, codigo, nombre FROM compania WHERE id_compania = @id_compania;";
            return connection.QueryFirstOrDefault<Compania>(sql, new { id_compania });
        }

        public bool ExisteCodigo(string codigo, int? excluirId = null)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT COUNT(*) FROM compania WHERE codigo = @codigo AND (@excluirId IS NULL OR id_compania != @excluirId);";
            return connection.ExecuteScalar<int>(sql, new { codigo, excluirId }) > 0;
        }

        public int Crear(Compania compania)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                INSERT INTO compania(codigo, nombre) VALUES(@codigo, @nombre);
                SELECT LAST_INSERT_ID();";
            return connection.ExecuteScalar<int>(sql, compania);
        }

        public void Actualizar(Compania compania)
        {
            using var connection = _context.CreateConnection();
            connection.Execute("UPDATE compania SET codigo = @codigo, nombre = @nombre WHERE id_compania = @id_compania;", compania);
        }

        public bool PuedeEliminar(int id_compania)
        {
            // No hay FK hacia compania en este modelo de datos
            return true;
        }

        public void Eliminar(int id_compania)
        {
            using var connection = _context.CreateConnection();
            connection.Execute("DELETE FROM compania WHERE id_compania = @id_compania;", new { id_compania });
        }
    }
}
