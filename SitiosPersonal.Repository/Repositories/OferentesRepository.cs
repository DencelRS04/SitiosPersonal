using Dapper;
using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Data;

namespace SitiosPersonal.Repository.Repositories
{
    public class OferentesRepository
    {
        private readonly DbContext _context;

        public OferentesRepository(DbContext context)
        {
            _context = context;
        }

        public List<OferenteListaItem> ListarPaginado(int pagina, int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string sql = @"
                SELECT id_oferente, identificacion, tipo_identificacion,
                       nombre_completo, fecha_nacimiento
                FROM oferente
                ORDER BY id_oferente DESC
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<OferenteListaItem>(sql, new { cantidadPorPagina, offset }).ToList();
        }

        public int Contar()
        {
            using var connection = _context.CreateConnection();

            string sql = "SELECT COUNT(*) FROM oferente;";

            return connection.ExecuteScalar<int>(sql);
        }

        public Oferente ObtenerPorId(int id_oferente)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_oferente, identificacion, tipo_identificacion,
                       nombre_completo, fecha_nacimiento, fecha_registro
                FROM oferente
                WHERE id_oferente = @id_oferente;";

            return connection.QueryFirstOrDefault<Oferente>(sql, new { id_oferente });
        }

        public List<string> ObtenerCorreos(int id_oferente)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT correo
                FROM oferente_correo
                WHERE id_oferente = @id_oferente
                ORDER BY id_correo;";

            return connection.Query<string>(sql, new { id_oferente }).ToList();
        }

        public List<string> ObtenerTelefonos(int id_oferente)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT telefono
                FROM oferente_telefono
                WHERE id_oferente = @id_oferente
                ORDER BY id_telefono;";

            return connection.Query<string>(sql, new { id_oferente }).ToList();
        }

        public List<int> ObtenerConcursos(int id_oferente)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_concurso
                FROM oferente_concurso
                WHERE id_oferente = @id_oferente;";

            return connection.Query<int>(sql, new { id_oferente }).ToList();
        }

        public List<Concurso> ListarConcursos()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT id_concurso, codigo, nombre, fecha_inicio, fecha_fin, estado
                FROM concurso
                ORDER BY nombre;";

            return connection.Query<Concurso>(sql).ToList();
        }

        public bool ExisteIdentificacion(string identificacion, int? idExcluir = null)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM oferente
                WHERE identificacion = @identificacion
                  AND (@idExcluir IS NULL OR id_oferente <> @idExcluir);";

            return connection.ExecuteScalar<int>(sql, new { identificacion, idExcluir }) > 0;
        }

        public int Crear(Oferente oferente, List<string> correos, List<string> telefonos, List<int> concursos)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlOferente = @"
                    INSERT INTO oferente(identificacion, tipo_identificacion, nombre_completo, fecha_nacimiento)
                    VALUES(@identificacion, @tipo_identificacion, @nombre_completo, @fecha_nacimiento);
                    SELECT LAST_INSERT_ID();";

                int idOferente = connection.ExecuteScalar<int>(sqlOferente, oferente, transaction);

                InsertarCorreos(connection, transaction, idOferente, correos);
                InsertarTelefonos(connection, transaction, idOferente, telefonos);
                InsertarConcursos(connection, transaction, idOferente, concursos);

                transaction.Commit();
                return idOferente;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Actualizar(Oferente oferente, List<string> correos, List<string> telefonos, List<int> concursos)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlOferente = @"
                    UPDATE oferente
                    SET identificacion      = @identificacion,
                        tipo_identificacion = @tipo_identificacion,
                        nombre_completo     = @nombre_completo,
                        fecha_nacimiento    = @fecha_nacimiento
                    WHERE id_oferente = @id_oferente;";

                connection.Execute(sqlOferente, oferente, transaction);

                connection.Execute(
                    "DELETE FROM oferente_correo WHERE id_oferente = @id_oferente;",
                    new { oferente.id_oferente }, transaction);

                connection.Execute(
                    "DELETE FROM oferente_telefono WHERE id_oferente = @id_oferente;",
                    new { oferente.id_oferente }, transaction);

                connection.Execute(
                    "DELETE FROM oferente_concurso WHERE id_oferente = @id_oferente;",
                    new { oferente.id_oferente }, transaction);

                InsertarCorreos(connection, transaction, oferente.id_oferente, correos);
                InsertarTelefonos(connection, transaction, oferente.id_oferente, telefonos);
                InsertarConcursos(connection, transaction, oferente.id_oferente, concursos);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Eliminar(int id_oferente)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                connection.Execute(
                    "DELETE FROM oferente_concurso WHERE id_oferente = @id_oferente;",
                    new { id_oferente }, transaction);

                // oferente_correo y oferente_telefono tienen ON DELETE CASCADE

                connection.Execute(
                    "DELETE FROM oferente WHERE id_oferente = @id_oferente;",
                    new { id_oferente }, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public bool PuedeEliminar(int id_oferente)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*) FROM empleado WHERE id_oferente = @id_oferente
                UNION ALL
                SELECT COUNT(*) FROM entrevista WHERE id_oferente = @id_oferente;";

            var counts = connection.Query<int>(sql, new { id_oferente }).ToList();
            return counts.All(c => c == 0);
        }

        private void InsertarCorreos(
            System.Data.IDbConnection connection,
            System.Data.IDbTransaction transaction,
            int idOferente,
            List<string> correos)
        {
            string sql = @"
                INSERT INTO oferente_correo(id_oferente, correo)
                VALUES(@id_oferente, @correo);";

            foreach (var correo in correos.Where(c => !string.IsNullOrWhiteSpace(c)))
            {
                connection.Execute(sql, new { id_oferente = idOferente, correo }, transaction);
            }
        }

        private void InsertarTelefonos(
            System.Data.IDbConnection connection,
            System.Data.IDbTransaction transaction,
            int idOferente,
            List<string> telefonos)
        {
            string sql = @"
                INSERT INTO oferente_telefono(id_oferente, telefono)
                VALUES(@id_oferente, @telefono);";

            foreach (var telefono in telefonos.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                connection.Execute(sql, new { id_oferente = idOferente, telefono }, transaction);
            }
        }

        private void InsertarConcursos(
            System.Data.IDbConnection connection,
            System.Data.IDbTransaction transaction,
            int idOferente,
            List<int> concursos)
        {
            if (concursos == null || !concursos.Any()) return;

            string sql = @"
                INSERT INTO oferente_concurso(id_oferente, id_concurso)
                VALUES(@id_oferente, @id_concurso);";

            foreach (var idConcurso in concursos)
            {
                connection.Execute(sql, new { id_oferente = idOferente, id_concurso = idConcurso }, transaction);
            }
        }
    }
}
