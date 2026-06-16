using System.ComponentModel.DataAnnotations;

namespace SitiosPersonal.Entities.ViewModels
{
    public class RequisitoPuestoViewModel
    {
        public int id_requisito { get; set; }

        public int id_puesto { get; set; }

        public string NombrePuesto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del requisito es obligatorio")]
        [StringLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres")]
        public string nombre { get; set; } = string.Empty;
    }

    public class RequisitoPuestoListaViewModel
    {
        public List<RequisitoPuestoListaItem> Requisitos { get; set; } = new List<RequisitoPuestoListaItem>();

        public int id_puesto { get; set; }

        public string NombrePuesto { get; set; } = string.Empty;

        public int Pagina { get; set; }

        public int CantidadPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / CantidadPorPagina);
    }

    public class RequisitoPuestoListaItem
    {
        public int id_requisito { get; set; }

        public int id_puesto { get; set; }

        public string nombre { get; set; } = string.Empty;
    }
}
