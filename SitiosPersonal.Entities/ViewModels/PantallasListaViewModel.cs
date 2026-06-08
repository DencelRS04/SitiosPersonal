using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Entities.ViewModels
{
    public class PantallasListaViewModel
    {
        public List<Pantalla> Pantallas { get; set; } = new List<Pantalla>();

        public int Pagina { get; set; }

        public int CantidadPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas
        {
            get
            {
                return (int)Math.Ceiling((double)TotalRegistros / CantidadPorPagina);
            }
        }
    }
}
