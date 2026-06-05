using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Entities.ViewModels
{
    public class PreparacionAcademicaListaViewModel
    {
        public int id_oferente { get; set; }

        public string NombreOferente { get; set; }

        public List<PreparacionAcademicaItem> Registros { get; set; } = new List<PreparacionAcademicaItem>();
    }

    public class PreparacionAcademicaItem
    {
        public int id_preparacion { get; set; }

        public int id_oferente { get; set; }

        public string NombreInstitucion { get; set; }

        public string titulo { get; set; }

        public DateTime fecha_inicio { get; set; }

        public DateTime fecha_fin { get; set; }
    }
}
