using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Entities.ViewModels
{
    public class UsuariosListaViewModel
    {
        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();

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