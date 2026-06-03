using SitiosPersonal.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class PantallaViewModel
    {
        public int id_pantalla { get; set; }

        [Required(ErrorMessage = "El nombre de la pantalla es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string nombre { get; set; }

        public List<int> RolesSeleccionados { get; set; } = new List<int>();

        public List<Rol> RolesDisponibles { get; set; } = new List<Rol>();
    }
}