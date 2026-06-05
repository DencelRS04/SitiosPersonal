using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class ExperienciaLaboralViewModel
    {
        public int id_experiencia { get; set; }

        public int id_oferente { get; set; }

        public string NombreOferente { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre de la empresa no puede superar los 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9\s]+$",
            ErrorMessage = "El nombre de la empresa solo puede contener letras, números y espacios")]
        public string empresa { get; set; }

        [Required(ErrorMessage = "El puesto desempeñado es obligatorio")]
        [StringLength(100, ErrorMessage = "El puesto desempeñado no puede superar los 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9\s]+$",
            ErrorMessage = "El puesto desempeñado solo puede contener letras, números y espacios")]
        public string puesto { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime? fecha_inicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime? fecha_fin { get; set; }
    }
}
