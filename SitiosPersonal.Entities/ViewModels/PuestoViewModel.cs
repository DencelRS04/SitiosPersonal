using SitiosPersonal.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class PuestoViewModel
    {
        public int id_puesto { get; set; }

        [Required(ErrorMessage = "El código del puesto es obligatorio")]
        [StringLength(20, ErrorMessage = "El código no puede superar los 20 caracteres")]
        public string codigo { get; set; }

        [Required(ErrorMessage = "El nombre del puesto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El monto del salario es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto del salario debe ser mayor a 0")]
        public decimal monto_salario { get; set; }

        public int? id_puesto_jefatura { get; set; }

        // Para el dropdown de jefatura
        public List<Puesto> PuestosDisponibles { get; set; } = new List<Puesto>();
    }

    public class PuestosListaViewModel
    {
        public List<PuestoListaItem> Puestos { get; set; } = new List<PuestoListaItem>();

        public int Pagina { get; set; }

        public int CantidadPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / CantidadPorPagina);
    }

    public class PuestoListaItem
    {
        public int id_puesto { get; set; }

        public string codigo { get; set; }

        public string nombre { get; set; }

        public decimal monto_salario { get; set; }

        public string NombreJefatura { get; set; }
    }
}
