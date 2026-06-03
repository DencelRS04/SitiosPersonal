using SitiosPersonal.Repository.Repositories;
using System.Text.Json;

namespace SitiosPersonal.Services.Services
{
    public class BitacoraService
    {
        private readonly BitacoraRepository _repository;

        public BitacoraService(BitacoraRepository repository)
        {
            _repository = repository;
        }

        public void RegistrarConsulta(
            int? idUsuario,
            string entidad)
        {
            _repository.Registrar(
                idUsuario,
                "SELECT",
                entidad,
                $"El usuario consulta {entidad}"
            );
        }

        public void RegistrarInsert(
            int? idUsuario,
            string entidad,
            object datosNuevos)
        {
            string jsonNuevo =
                JsonSerializer.Serialize(datosNuevos);

            _repository.Registrar(
                idUsuario,
                "INSERT",
                entidad,
                $"El usuario registra {entidad}. Datos nuevos: {jsonNuevo}",
                null,
                jsonNuevo
            );
        }

        public void RegistrarUpdate(
            int? idUsuario,
            string entidad,
            object datosAnteriores,
            object datosNuevos)
        {
            string jsonAnterior =
                JsonSerializer.Serialize(datosAnteriores);

            string jsonNuevo =
                JsonSerializer.Serialize(datosNuevos);

            _repository.Registrar(
                idUsuario,
                "UPDATE",
                entidad,
                $"El usuario actualiza {entidad}. Datos anteriores: {jsonAnterior}. Datos nuevos: {jsonNuevo}",
                jsonAnterior,
                jsonNuevo
            );
        }

        public void RegistrarDelete(
            int? idUsuario,
            string entidad,
            object datosEliminados)
        {
            string jsonEliminado =
                JsonSerializer.Serialize(datosEliminados);

            _repository.Registrar(
                idUsuario,
                "DELETE",
                entidad,
                $"El usuario elimina {entidad}. Datos eliminados: {jsonEliminado}",
                jsonEliminado,
                null
            );
        }

        public void RegistrarError(
            int? idUsuario,
            string entidad,
            string error)
        {
            _repository.Registrar(
                idUsuario,
                "ERROR",
                entidad,
                $"Error técnico en {entidad}: {error}"
            );
        }
    }
}