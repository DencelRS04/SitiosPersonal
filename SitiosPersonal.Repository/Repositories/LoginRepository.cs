using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class LoginRepository
    {
        private readonly DbContext _context;

        public LoginRepository(DbContext context)
        {
            _context = context;
        }

        public Usuario ObtenerUsuario(string usuario)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT
                    id_usuario,
                    usuario,
                    nombre_completo,
                    correo,
                    password_hash,
                    estado,
                    intentos_login
                FROM usuario
                WHERE usuario = @usuario;";

            return connection.QueryFirstOrDefault<Usuario>(
                sql,
                new { usuario }
            );
        }

        public void AumentarIntentos(int id_usuario)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE usuario
                SET intentos_login = intentos_login + 1
                WHERE id_usuario = @id_usuario;";

            connection.Execute(
                sql,
                new { id_usuario }
            );
        }

        public void BloquearUsuario(int id_usuario)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE usuario
                SET estado = 'BLOQUEADO',
                    fecha_bloqueo = NOW()
                WHERE id_usuario = @id_usuario;";

            connection.Execute(
                sql,
                new { id_usuario }
            );
        }

        public void ReiniciarIntentos(int id_usuario)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE usuario
                SET intentos_login = 0,
                    fecha_ultimo_login = NOW()
                WHERE id_usuario = @id_usuario;";

            connection.Execute(
                sql,
                new { id_usuario }
            );
        }
    }
}
