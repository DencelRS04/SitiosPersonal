using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class ConcursoViewModel
    {
        public int id_concurso { get; set; }

        [Required(ErrorMessage = "El código del concurso es obligatorio")]
        [StringLength(50, ErrorMessage = "El código no puede superar los 50 caracteres")]
        public string codigo { get; set; }

        [Required(ErrorMessage = "El nombre del concurso es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime? fecha_inicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime? fecha_fin { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string estado { get; set; } = "VIGENTE";
    }
}
