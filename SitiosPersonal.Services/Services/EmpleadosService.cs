using SitiosPersonal.Entities.Models;
using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class EmpleadosService
    {
        private readonly EmpleadosRepository _empleadosRepo;

        public EmpleadosService(EmpleadosRepository empleadosRepo)
        {
            _empleadosRepo = empleadosRepo;
        }

        public EmpleadosListaViewModel ObtenerPaginado(int pagina, int cantidadPorPagina)
        {
            return new EmpleadosListaViewModel
            {
                Empleados = _empleadosRepo.ListarPaginado(pagina, cantidadPorPagina),
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _empleadosRepo.Contar()
            };
        }

        public ContratarEmpleadoViewModel ObtenerFormularioContratar()
        {
            return new ContratarEmpleadoViewModel
            {
                OferentesDisponibles = _empleadosRepo.ListarOferentesDisponibles(),
                PuestosDisponibles = _empleadosRepo.ListarPuestos()
            };
        }

        public (bool exito, string error) Contratar(ContratarEmpleadoViewModel vm)
        {
            if (_empleadosRepo.OferenteYaEsEmpleado(vm.id_oferente!.Value))
                return (false, "Este oferente ya ha sido contratado como empleado.");

            string numeroEmpleado = _empleadosRepo.GenerarNumeroEmpleado();

            var empleado = new Empleado
            {
                numero_empleado = numeroEmpleado,
                id_oferente = vm.id_oferente.Value,
                id_puesto = vm.id_puesto!.Value,
                fecha_contratacion = DateTime.Now,
                estado = "ACTIVO"
            };

            _empleadosRepo.Contratar(empleado);

            return (true, null);
        }

        public List<EmpleadoDropdownItem> ListarDropdown() =>
            _empleadosRepo.ListarTodosDropdown();
    }
}
