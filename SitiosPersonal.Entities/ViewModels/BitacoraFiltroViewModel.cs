using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Entities.ViewModels
{
    public class BitacoraFiltroViewModel
    {
        public string UsuarioFiltro { get; set; }

        public string DescripcionFiltro { get; set; }

        public string Orden { get; set; } = "fecha_desc";

        public int Pagina { get; set; } = 1;

        public int TotalRegistros { get; set; }

        public int CantidadPorPagina { get; set; } = 100;

        public List<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
    }
}