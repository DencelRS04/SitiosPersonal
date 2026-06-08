using SitiosPersonal.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class ContratarEmpleadoViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un oferente")]
        public int? id_oferente { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un puesto")]
        public int? id_puesto { get; set; }

        // Listas para dropdowns
        public List<OferenteDropdownItem> OferentesDisponibles { get; set; } = new List<OferenteDropdownItem>();

        public List<PuestoDropdownItem> PuestosDisponibles { get; set; } = new List<PuestoDropdownItem>();
    }

    public class PuestoDropdownItem
    {
        public int id_puesto { get; set; }

        public string codigo { get; set; }

        public string nombre { get; set; }
    }

    public class EmpleadoListaItem
    {
        public int id_empleado { get; set; }

        public string numero_empleado { get; set; }

        public string nombre_completo { get; set; }

        public string identificacion { get; set; }

        public string NombrePuesto { get; set; }

        public DateTime fecha_contratacion { get; set; }

        public string estado { get; set; }
    }

    public class EmpleadosListaViewModel
    {
        public List<EmpleadoListaItem> Empleados { get; set; } = new List<EmpleadoListaItem>();

        public int Pagina { get; set; }

        public int CantidadPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / CantidadPorPagina);
    }
}
