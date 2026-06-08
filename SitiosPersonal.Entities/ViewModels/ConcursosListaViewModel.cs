using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Entities.ViewModels
{
    public class ConcursosListaViewModel
    {
        public List<Concurso> Concursos { get; set; } = new List<Concurso>();

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
