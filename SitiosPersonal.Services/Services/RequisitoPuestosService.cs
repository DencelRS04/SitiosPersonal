using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class RequisitoPuestosService
    {
        private readonly RequisitosPuestoRepository _requisitoRepo;
        private readonly PuestosRepository _puestosRepo;

        public RequisitoPuestosService(RequisitosPuestoRepository requisitoRepo, PuestosRepository puestosRepo)
        {
            _requisitoRepo = requisitoRepo;
            _puestosRepo = puestosRepo;
        }

        public RequisitoPuestoListaViewModel ObtenerPaginado(int idPuesto, int pagina, int cantidadPorPagina)
        {
            var puesto = _puestosRepo.ObtenerPorId(idPuesto);
            if (puesto == null) return null;

            return new RequisitoPuestoListaViewModel
            {
                Requisitos = _requisitoRepo.ListarPorPuesto(idPuesto, pagina, cantidadPorPagina),
                id_puesto = idPuesto,
                NombrePuesto = puesto.nombre,
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _requisitoRepo.ContarPorPuesto(idPuesto)
            };
        }

        public RequisitoPuestoViewModel ObtenerFormularioCrear(int idPuesto)
        {
            var puesto = _puestosRepo.ObtenerPorId(idPuesto);
            if (puesto == null) return null;

            return new RequisitoPuestoViewModel
            {
                id_puesto = idPuesto,
                NombrePuesto = puesto.nombre
            };
        }

        public RequisitoPuestoViewModel ObtenerFormularioEditar(int idRequisito)
        {
            var requisito = _requisitoRepo.ObtenerPorId(idRequisito);
            if (requisito == null) return null;

            var puesto = _puestosRepo.ObtenerPorId(requisito.id_puesto);

            return new RequisitoPuestoViewModel
            {
                id_requisito = requisito.id_requisito,
                id_puesto = requisito.id_puesto,
                nombre = requisito.nombre,
                NombrePuesto = puesto?.nombre
            };
        }

        public void Crear(RequisitoPuestoViewModel vm)
        {
            var requisito = new RequisitoPuesto
            {
                id_puesto = vm.id_puesto,
                nombre = vm.nombre
            };

            _requisitoRepo.Crear(requisito);
        }

        public void Actualizar(RequisitoPuestoViewModel vm)
        {
            var requisito = new RequisitoPuesto
            {
                id_requisito = vm.id_requisito,
                id_puesto = vm.id_puesto,
                nombre = vm.nombre
            };

            _requisitoRepo.Actualizar(requisito);
        }

        public (bool exito, string error) Eliminar(int id)
        {
            if (!_requisitoRepo.PuedeEliminar(id))
                return (false, "No se puede eliminar un registro con datos relacionados.");

            _requisitoRepo.Eliminar(id);
            return (true, null);
        }
    }
}
