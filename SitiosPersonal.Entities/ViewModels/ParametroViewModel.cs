using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class ParametroViewModel
    {
        public int id_parametro { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(100, ErrorMessage = "El código no puede superar los 100 caracteres")]
        public string codigo { get; set; }

        [Required(ErrorMessage = "El valor es obligatorio")]
        [StringLength(500, ErrorMessage = "El valor no puede superar los 500 caracteres")]
        public string valor { get; set; }
    }
}
