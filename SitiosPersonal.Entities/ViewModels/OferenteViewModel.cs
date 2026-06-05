using SitiosPersonal.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class OferenteViewModel
    {
        public int id_oferente { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [StringLength(30, ErrorMessage = "La identificación no puede superar los 30 caracteres")]
        public string identificacion { get; set; }

        [Required(ErrorMessage = "El tipo de identificación es obligatorio")]
        public string tipo_identificacion { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres")]
        public string nombre_completo { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime? fecha_nacimiento { get; set; }

        public List<string> Correos { get; set; } = new List<string> { "" };

        public List<string> Telefonos { get; set; } = new List<string> { "" };

        public List<int> ConcursosSeleccionados { get; set; } = new List<int>();

        public List<Concurso> ConcursosDisponibles { get; set; } = new List<Concurso>();
    }
}
