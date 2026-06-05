namespace SitiosPersonal.Entities.ViewModels
{
    public class ExperienciaLaboralListaViewModel
    {
        public int id_oferente { get; set; }

        public string NombreOferente { get; set; }

        public List<ExperienciaLaboralItem> Registros { get; set; } = new List<ExperienciaLaboralItem>();
    }

    public class ExperienciaLaboralItem
    {
        public int id_experiencia { get; set; }

        public int id_oferente { get; set; }

        public string empresa { get; set; }

        public string puesto { get; set; }

        public DateTime fecha_inicio { get; set; }

        public DateTime fecha_fin { get; set; }
    }
}
