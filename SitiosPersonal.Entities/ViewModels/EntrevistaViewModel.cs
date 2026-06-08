using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class EntrevistaViewModel
    {
        public int id_entrevista { get; set; }

        [Required(ErrorMessage = "El oferente es obligatorio")]
        public int? id_oferente { get; set; }

        [Required(ErrorMessage = "El empleado entrevistador es obligatorio")]
        public int? id_empleado_entrevistador { get; set; }

        [Required(ErrorMessage = "La fecha de la entrevista es obligatoria")]
        public DateTime? fecha_entrevista { get; set; }

        [Required(ErrorMessage = "La hora de la entrevista es obligatoria")]
        public TimeSpan? hora_entrevista { get; set; }

        public string estado { get; set; } = "PENDIENTE";

        // Datos de solo lectura para mostrar en Editar (no se envía en el POST)
        public string? NombreOferente { get; set; }

        // Listas para dropdowns
        public List<OferenteDropdownItem> OferentesDisponibles { get; set; } = new List<OferenteDropdownItem>();

        public List<EmpleadoDropdownItem> EmpleadosDisponibles { get; set; } = new List<EmpleadoDropdownItem>();
    }

    public class OferenteDropdownItem
    {
        public int id_oferente { get; set; }

        public string nombre_completo { get; set; }
    }

    public class EmpleadoDropdownItem
    {
        public int id_empleado { get; set; }

        public string numero_empleado { get; set; }

        public string nombre_completo { get; set; }
    }
}
