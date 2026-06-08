using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class UbicacionRepository
    {
        private readonly DbContext _context;

        public UbicacionRepository(DbContext context)
        {
            _context = context;
        }

        public void UpsertProvincia(Provincia provincia)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                INSERT INTO provincia(id_provincia, nombre)
                VALUES(@id_provincia, @nombre)
                ON DUPLICATE KEY UPDATE nombre = @nombre;";
            connection.Execute(sql, provincia);
        }

        public void UpsertCanton(Canton canton)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                INSERT INTO canton(id_canton, id_provincia, nombre)
                VALUES(@id_canton, @id_provincia, @nombre)
                ON DUPLICATE KEY UPDATE nombre = @nombre;";
            connection.Execute(sql, canton);
        }

        public void UpsertDistrito(Distrito distrito)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                INSERT INTO distrito(id_distrito, id_canton, nombre)
                VALUES(@id_distrito, @id_canton, @nombre)
                ON DUPLICATE KEY UPDATE nombre = @nombre;";
            connection.Execute(sql, distrito);
        }

        public IEnumerable<Provincia> ListarProvincias()
        {
            using var connection = _context.CreateConnection();
            return connection.Query<Provincia>("SELECT * FROM provincia ORDER BY nombre");
        }

        public IEnumerable<dynamic> ListarCantones()
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                SELECT c.id_canton, c.nombre, p.nombre AS nombre_provincia
                FROM canton c
                INNER JOIN provincia p ON p.id_provincia = c.id_provincia
                ORDER BY p.nombre, c.nombre";
            return connection.Query(sql);
        }

        public IEnumerable<dynamic> ListarDistritos()
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                SELECT d.id_distrito, d.nombre, c.nombre AS nombre_canton, p.nombre AS nombre_provincia
                FROM distrito d
                INNER JOIN canton c ON c.id_canton = d.id_canton
                INNER JOIN provincia p ON p.id_provincia = c.id_provincia
                ORDER BY p.nombre, c.nombre, d.nombre";
            return connection.Query(sql);
        }
    }
}
