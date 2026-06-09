using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class PuestosService
    {
        private readonly PuestosRepository _puestosRepo;

        public PuestosService(PuestosRepository puestosRepo)
        {
            _puestosRepo = puestosRepo;
        }

        public PuestosListaViewModel ObtenerPaginado(int pagina, int cantidadPorPagina)
        {
            return new PuestosListaViewModel
            {
                Puestos = _puestosRepo.ListarPaginado(pagina, cantidadPorPagina),
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _puestosRepo.Contar()
            };
        }

        public PuestoViewModel ObtenerFormularioCrear()
        {
            return new PuestoViewModel
            {
                PuestosDisponibles = _puestosRepo.ListarTodos()
            };
        }

        public PuestoViewModel ObtenerFormularioEditar(int id)
        {
            var puesto = _puestosRepo.ObtenerPorId(id);
            if (puesto == null) return null;

            return new PuestoViewModel
            {
                id_puesto = puesto.id_puesto,
                codigo = puesto.codigo,
                nombre = puesto.nombre,
                monto_salario = puesto.monto_salario,
                id_puesto_jefatura = puesto.id_puesto_jefatura,
                PuestosDisponibles = _puestosRepo.ListarTodos().Where(p => p.id_puesto != id).ToList()
            };
        }

        public (bool exito, string error) Crear(PuestoViewModel vm)
        {
            if (_puestosRepo.ExisteCodigo(vm.codigo))
                return (false, "Ya existe un puesto con este código.");

            var puesto = new Puesto
            {
                codigo = vm.codigo,
                nombre = vm.nombre,
                monto_salario = vm.monto_salario,
                id_puesto_jefatura = vm.id_puesto_jefatura,
                activo = true
            };

            _puestosRepo.Crear(puesto);
            return (true, null);
        }

        public (bool exito, string error) Actualizar(PuestoViewModel vm)
        {
            if (_puestosRepo.ExisteCodigo(vm.codigo, vm.id_puesto))
                return (false, "Ya existe un puesto con este código.");

            var puesto = new Puesto
            {
                id_puesto = vm.id_puesto,
                codigo = vm.codigo,
                nombre = vm.nombre,
                monto_salario = vm.monto_salario,
                id_puesto_jefatura = vm.id_puesto_jefatura
            };

            _puestosRepo.Actualizar(puesto);
            return (true, null);
        }

        public (bool exito, string error) Eliminar(int id)
        {
            if (!_puestosRepo.PuedeEliminar(id))
                return (false, "No se puede eliminar un puesto con datos relacionados.");

            _puestosRepo.Eliminar(id);
            return (true, null);
        }

        public Puesto ObtenerPorId(int id) => _puestosRepo.ObtenerPorId(id);

        public List<Puesto> ListarTodos() => _puestosRepo.ListarTodos();
    }
}
