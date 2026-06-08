using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class AreasService
    {
        private readonly AreasRepository _areasRepo;
        private readonly EmpleadosRepository _empleadosRepo;

        public AreasService(AreasRepository areasRepo, EmpleadosRepository empleadosRepo)
        {
            _areasRepo = areasRepo;
            _empleadosRepo = empleadosRepo;
        }

        public AreasListaViewModel ObtenerPaginado(int pagina, int cantidadPorPagina)
        {
            return new AreasListaViewModel
            {
                Areas = _areasRepo.ListarPaginado(pagina, cantidadPorPagina),
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _areasRepo.Contar()
            };
        }

        public AreaViewModel ObtenerFormularioCrear()
        {
            return new AreaViewModel
            {
                EmpleadosDisponibles = _empleadosRepo.ListarTodosDropdown()
            };
        }

        public AreaViewModel ObtenerFormularioEditar(int id)
        {
            var area = _areasRepo.ObtenerPorId(id);
            if (area == null) return null;

            return new AreaViewModel
            {
                id_area = area.id_area,
                codigo = area.codigo,
                nombre = area.nombre,
                id_empleado_jefatura = area.id_empleado_jefatura,
                EmpleadosDisponibles = _empleadosRepo.ListarTodosDropdown()
            };
        }

        public (bool exito, string error) Crear(AreaViewModel vm)
        {
            if (_areasRepo.ExisteCodigo(vm.codigo))
                return (false, "Ya existe un área con este código.");

            var area = new Area
            {
                codigo = vm.codigo,
                nombre = vm.nombre,
                id_empleado_jefatura = vm.id_empleado_jefatura
            };

            _areasRepo.Crear(area);
            return (true, null);
        }

        public (bool exito, string error) Actualizar(AreaViewModel vm)
        {
            if (_areasRepo.ExisteCodigo(vm.codigo, vm.id_area))
                return (false, "Ya existe un área con este código.");

            var area = new Area
            {
                id_area = vm.id_area,
                codigo = vm.codigo,
                nombre = vm.nombre,
                id_empleado_jefatura = vm.id_empleado_jefatura
            };

            _areasRepo.Actualizar(area);
            return (true, null);
        }

        public (bool exito, string error) Eliminar(int id)
        {
            if (!_areasRepo.PuedeEliminar(id))
                return (false, "No se puede eliminar un área con datos relacionados.");

            _areasRepo.Eliminar(id);
            return (true, null);
        }
    }
}
