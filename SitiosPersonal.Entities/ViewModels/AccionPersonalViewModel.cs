using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class AccionPersonalViewModel
    {
        public int id_accion { get; set; }

        public string codigo { get; set; }

        [Required(ErrorMessage = "La fecha de la acción es obligatoria")]
        public DateTime? fecha_accion { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "El empleado es obligatorio")]
        public int? id_empleado { get; set; }

        [Required(ErrorMessage = "La jefatura que aprueba es obligatoria")]
        public int? id_empleado_jefatura { get; set; }

        public List<EmpleadoDropdownItem> EmpleadosDisponibles { get; set; } = new List<EmpleadoDropdownItem>();
    }

    public class AccionesPersonalListaViewModel
    {
        public List<AccionPersonalListaItem> Acciones { get; set; } = new List<AccionPersonalListaItem>();

        public int Pagina { get; set; }

        public int CantidadPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / CantidadPorPagina);
    }

    public class AccionPersonalListaItem
    {
        public int id_accion { get; set; }

        public string codigo { get; set; }

        public DateTime fecha_accion { get; set; }

        public string descripcion { get; set; }

        public string NombreEmpleado { get; set; }

        public string NombreJefatura { get; set; }
    }
}
