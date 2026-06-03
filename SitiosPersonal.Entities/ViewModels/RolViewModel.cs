using SitiosPersonal.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class RolViewModel
    {
        public int id_rol { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(40, ErrorMessage = "El nombre no puede superar los 40 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string nombre { get; set; }

        public bool activo { get; set; } = true;

        public List<int> PantallasSeleccionadas { get; set; } = new List<int>();

        public List<Pantalla> PantallasDisponibles { get; set; } = new List<Pantalla>();
    }
}