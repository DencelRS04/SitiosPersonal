using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Entities.ViewModels
{
    public class CompaniasListaViewModel
    {
        public List<Compania> Companias { get; set; } = new List<Compania>();
        public int Pagina { get; set; }
        public int CantidadPorPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / CantidadPorPagina);
    }
}
