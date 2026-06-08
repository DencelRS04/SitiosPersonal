using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Usuario y/o contraseña incorrectos.")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Usuario y/o contraseña incorrectos.")]
        public string Password { get; set; }
    }
}
