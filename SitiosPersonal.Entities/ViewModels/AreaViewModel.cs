using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class AreaViewModel
    {
        public int id_area { get; set; }

        [Required(ErrorMessage = "El código del área es obligatorio")]
        [StringLength(20, ErrorMessage = "El código no puede superar los 20 caracteres")]
        public string codigo { get; set; }

        [Required(ErrorMessage = "El nombre del área es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$", ErrorMessage = "El nombre solo debe contener letras y espacios")]
        public string nombre { get; set; }

        public int? id_empleado_jefatura { get; set; }

        public List<EmpleadoDropdownItem> EmpleadosDisponibles { get; set; } = new List<EmpleadoDropdownItem>();
    }

    public class AreasListaViewModel
    {
        public List<AreaListaItem> Areas { get; set; } = new List<AreaListaItem>();

        public int Pagina { get; set; }

        public int CantidadPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / CantidadPorPagina);
    }

    public class AreaListaItem
    {
        public int id_area { get; set; }

        public string codigo { get; set; }

        public string nombre { get; set; }

        public string NombreJefatura { get; set; }
    }
}
