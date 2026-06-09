using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class ParametroRepository
    {
        private readonly DbContext _context;

        public ParametroRepository(DbContext context)
        {
            _context = context;
        }

        public List<Parametro> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();
            int offset = (pagina - 1) * cantidadPorPagina;
            string sql = @"
                SELECT id_parametro, codigo, valor
                FROM parametro
                ORDER BY id_parametro DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";
            return connection.Query<Parametro>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM parametro;");
        }

        public Parametro ObtenerPorId(int id_parametro)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT id_parametro, codigo, valor FROM parametro WHERE id_parametro = @id_parametro;";
            return connection.QueryFirstOrDefault<Parametro>(sql, new { id_parametro });
        }

        public bool ExisteCodigo(string codigo, int? excluirId = null)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT COUNT(*) FROM parametro WHERE codigo = @codigo AND (@excluirId IS NULL OR id_parametro != @excluirId);";
            return connection.ExecuteScalar<int>(sql, new { codigo, excluirId }) > 0;
        }

        public int Crear(Parametro parametro)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                INSERT INTO parametro(codigo, valor) VALUES(@codigo, @valor);
                SELECT LAST_INSERT_ID();";
            return connection.ExecuteScalar<int>(sql, parametro);
        }

        public void Actualizar(Parametro parametro)
        {
            using var connection = _context.CreateConnection();
            string sql = "UPDATE parametro SET codigo = @codigo, valor = @valor WHERE id_parametro = @id_parametro;";
            connection.Execute(sql, parametro);
        }

        public bool PuedeEliminar(int id_parametro)
        {
            // Parámetros son configuración general, sin FK directas en este modelo
            return true;
        }

        public void Eliminar(int id_parametro)
        {
            using var connection = _context.CreateConnection();
            connection.Execute("DELETE FROM parametro WHERE id_parametro = @id_parametro;", new { id_parametro });
        }
    }
}
