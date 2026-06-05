namespace SitiosPersonal.Entities.Models
{
    public class ExperienciaLaboral
    {
        public int id_experiencia { get; set; }

        public int id_oferente { get; set; }

        public string empresa { get; set; }

        public string puesto { get; set; }

        public DateTime fecha_inicio { get; set; }

        public DateTime fecha_fin { get; set; }
    }
}
