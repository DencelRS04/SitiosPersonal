using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Entities.ViewModels
{
    public class OferentesListaViewModel
    {
        public List<OferenteListaItem> Oferentes { get; set; } = new List<OferenteListaItem>();

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

    public class OferenteListaItem
    {
        public int id_oferente { get; set; }

        public string identificacion { get; set; }

        public string tipo_identificacion { get; set; }

        public string nombre_completo { get; set; }

        public DateTime fecha_nacimiento { get; set; }
    }
}
