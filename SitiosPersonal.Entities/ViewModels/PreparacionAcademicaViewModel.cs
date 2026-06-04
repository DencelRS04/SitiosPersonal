using SitiosPersonal.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class PreparacionAcademicaViewModel
    {
        public int id_preparacion { get; set; }

        public int id_oferente { get; set; }

        public string NombreOferente { get; set; }

        [Required(ErrorMessage = "La institución educativa es obligatoria")]
        public int? id_institucion { get; set; }

        [Required(ErrorMessage = "El nombre del título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$",
            ErrorMessage = "El título solo puede contener letras y espacios")]
        public string titulo { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime? fecha_inicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime? fecha_fin { get; set; }

        public List<InstitucionEducativa> InstitucionesDisponibles { get; set; } = new List<InstitucionEducativa>();
    }
}
