using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Entities.ViewModels
{
    public class RolesListaViewModel
    {
        public List<Rol> Roles { get; set; } = new List<Rol>();

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
