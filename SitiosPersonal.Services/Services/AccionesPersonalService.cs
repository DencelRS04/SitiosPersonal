using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class AccionesPersonalService
    {
        private readonly AccionesPersonalRepository _accionesRepo;
        private readonly EmpleadosRepository _empleadosRepo;

        public AccionesPersonalService(AccionesPersonalRepository accionesRepo, EmpleadosRepository empleadosRepo)
        {
            _accionesRepo = accionesRepo;
            _empleadosRepo = empleadosRepo;
        }

        public AccionesPersonalListaViewModel ObtenerPaginado(int pagina, int cantidadPorPagina)
        {
            return new AccionesPersonalListaViewModel
            {
                Acciones = _accionesRepo.ListarPaginado(pagina, cantidadPorPagina),
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _accionesRepo.Contar()
            };
        }

        public AccionPersonalViewModel ObtenerFormularioCrear()
        {
            return new AccionPersonalViewModel
            {
                codigo = _accionesRepo.GenerarCodigo(),
                fecha_accion = DateTime.Today,
                EmpleadosDisponibles = _empleadosRepo.ListarTodosDropdown()
            };
        }

        public AccionPersonalViewModel ObtenerFormularioEditar(int id)
        {
            var accion = _accionesRepo.ObtenerPorId(id);
            if (accion == null) return null;

            return new AccionPersonalViewModel
            {
                id_accion = accion.id_accion,
                codigo = accion.codigo,
                fecha_accion = accion.fecha_accion,
                descripcion = accion.descripcion,
                id_empleado = accion.id_empleado,
                id_empleado_jefatura = accion.id_empleado_jefatura,
                EmpleadosDisponibles = _empleadosRepo.ListarTodosDropdown()
            };
        }

        public void Crear(AccionPersonalViewModel vm)
        {
            var accion = new AccionPersonal
            {
                codigo = _accionesRepo.GenerarCodigo(),
                fecha_accion = vm.fecha_accion!.Value,
                descripcion = vm.descripcion,
                id_empleado = vm.id_empleado!.Value,
                id_empleado_jefatura = vm.id_empleado_jefatura!.Value
            };

            _accionesRepo.Crear(accion);
        }

        public void Actualizar(AccionPersonalViewModel vm)
        {
            var anterior = _accionesRepo.ObtenerPorId(vm.id_accion);

            var accion = new AccionPersonal
            {
                id_accion = vm.id_accion,
                codigo = anterior.codigo,
                fecha_accion = vm.fecha_accion!.Value,
                descripcion = vm.descripcion,
                id_empleado = vm.id_empleado!.Value,
                id_empleado_jefatura = vm.id_empleado_jefatura!.Value
            };

            _accionesRepo.Actualizar(accion);
        }

        public (bool exito, string error) Eliminar(int id)
        {
            if (!_accionesRepo.PuedeEliminar(id))
                return (false, "No se puede eliminar un registro con datos relacionados.");

            _accionesRepo.Eliminar(id);
            return (true, null);
        }
    }
}
