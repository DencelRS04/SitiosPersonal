using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Entities.ViewModels
{
    public class InstitucionesEducativasListaViewModel
    {
        public List<InstitucionEducativa> Instituciones { get; set; } = new List<InstitucionEducativa>();
        public int Pagina { get; set; }
        public int CantidadPorPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / CantidadPorPagina);
    }
}
