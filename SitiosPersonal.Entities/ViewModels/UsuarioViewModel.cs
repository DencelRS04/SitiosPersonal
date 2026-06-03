using SitiosPersonal.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class UsuarioViewModel
    {
        public int id_usuario { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string usuario { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        public string nombre_completo { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        public string correo { get; set; }

        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$",
            ErrorMessage = "La contraseña debe tener mínimo 8 caracteres, mayúsculas, minúsculas, números y caracteres especiales"
        )]
        public string? password { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string estado { get; set; }

        public List<int> RolesSeleccionados { get; set; } = new List<int>();

        public List<Rol> RolesDisponibles { get; set; } = new List<Rol>();
    }
}