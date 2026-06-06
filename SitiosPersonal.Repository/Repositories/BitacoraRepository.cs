using Dapper;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Repository.Repositories
{
    public class BitacoraRepository
    {
        private readonly DbContext _context;

        public BitacoraRepository(DbContext context)
        {
            _context = context;
        }

        public void Registrar(
            int? id_usuario,
            string tipo,
            string entidad,
            string descripcion,
            string datos_anteriores = null,
            string datos_nuevos = null)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO bitacora
                (
                    fecha,
                    id_usuario,
                    tipo,
                    entidad,
                    datos_anteriores,
                    datos_nuevos,
                    descripcion
                )
                VALUES
                (
                    NOW(),
                    @id_usuario,
                    @tipo,
                    @entidad,
                    @datos_anteriores,
                    @datos_nuevos,
                    @descripcion
                );";

            connection.Execute(sql, new
            {
                id_usuario,
                tipo,
                entidad,
                datos_anteriores,
                datos_nuevos,
                descripcion
            });
        }

        public List<Bitacora> Listar(
            string usuarioFiltro,
            string descripcionFiltro,
            string orden,
            int pagina,
            int cantidadPorPagina)
        {
            using var connection = _context.CreateConnection();

            int offset = (pagina - 1) * cantidadPorPagina;

            string orderBy = orden switch
            {
                "fecha_asc" => "b.fecha ASC",
                "usuario_asc" => "u.usuario ASC",
                "usuario_desc" => "u.usuario DESC",
                _ => "b.fecha DESC"
            };

            string sql = $@"
                SELECT
                    b.id_bitacora,
                    b.fecha,
                    b.id_usuario,
                    u.usuario,
                    b.tipo,
                    b.entidad,
                    CAST(b.datos_anteriores AS CHAR) AS datos_anteriores,
                    CAST(b.datos_nuevos AS CHAR) AS datos_nuevos,
                    b.descripcion
                FROM bitacora b
                LEFT JOIN usuario u
                    ON b.id_usuario = u.id_usuario
                WHERE
                    (@usuarioFiltro IS NULL OR u.usuario LIKE CONCAT('%', @usuarioFiltro, '%'))
                    AND
                    (@descripcionFiltro IS NULL OR b.descripcion LIKE CONCAT('%', @descripcionFiltro, '%'))
                ORDER BY {orderBy}
                LIMIT @cantidadPorPagina OFFSET @offset;";

            return connection.Query<Bitacora>(sql, new
            {
                usuarioFiltro = string.IsNullOrWhiteSpace(usuarioFiltro) ? null : usuarioFiltro,
                descripcionFiltro = string.IsNullOrWhiteSpace(descripcionFiltro) ? null : descripcionFiltro,
                cantidadPorPagina,
                offset
            }).ToList();
        }

        public int Contar(
            string usuarioFiltro,
            string descripcionFiltro)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                SELECT COUNT(*)
                FROM bitacora b
                LEFT JOIN usuario u
                    ON b.id_usuario = u.id_usuario
                WHERE
                    (@usuarioFiltro IS NULL OR u.usuario LIKE CONCAT('%', @usuarioFiltro, '%'))
                    AND
                    (@descripcionFiltro IS NULL OR b.descripcion LIKE CONCAT('%', @descripcionFiltro, '%'));";

            return connection.ExecuteScalar<int>(sql, new
            {
                usuarioFiltro = string.IsNullOrWhiteSpace(usuarioFiltro) ? null : usuarioFiltro,
                descripcionFiltro = string.IsNullOrWhiteSpace(descripcionFiltro) ? null : descripcionFiltro
            });
        }
    }
}
