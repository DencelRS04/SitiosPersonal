using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class InstitucionEducativaRepository
    {
        private readonly DbContext _context;

        public InstitucionEducativaRepository(DbContext context)
        {
            _context = context;
        }

        public List<InstitucionEducativa> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();
            int offset = (pagina - 1) * cantidadPorPagina;
            string sql = @"
                SELECT id_institucion, codigo, nombre
                FROM institucion_educativa
                ORDER BY id_institucion DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";
            return connection.Query<InstitucionEducativa>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();
            return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM institucion_educativa;");
        }

        public InstitucionEducativa ObtenerPorId(int id_institucion)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT id_institucion, codigo, nombre FROM institucion_educativa WHERE id_institucion = @id_institucion;";
            return connection.QueryFirstOrDefault<InstitucionEducativa>(sql, new { id_institucion });
        }

        public bool ExisteCodigo(string codigo, int? excluirId = null)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT COUNT(*) FROM institucion_educativa WHERE codigo = @codigo AND (@excluirId IS NULL OR id_institucion != @excluirId);";
            return connection.ExecuteScalar<int>(sql, new { codigo, excluirId }) > 0;
        }

        public int Crear(InstitucionEducativa institucion)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                INSERT INTO institucion_educativa(codigo, nombre) VALUES(@codigo, @nombre);
                SELECT LAST_INSERT_ID();";
            return connection.ExecuteScalar<int>(sql, institucion);
        }

        public void Actualizar(InstitucionEducativa institucion)
        {
            using var connection = _context.CreateConnection();
            connection.Execute("UPDATE institucion_educativa SET codigo = @codigo, nombre = @nombre WHERE id_institucion = @id_institucion;", institucion);
        }

        public bool PuedeEliminar(int id_institucion)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT COUNT(*) FROM preparacion_academica WHERE id_institucion = @id_institucion;";
            return connection.ExecuteScalar<int>(sql, new { id_institucion }) == 0;
        }

        public void Eliminar(int id_institucion)
        {
            using var connection = _context.CreateConnection();
            connection.Execute("DELETE FROM institucion_educativa WHERE id_institucion = @id_institucion;", new { id_institucion });
        }
    }
}
