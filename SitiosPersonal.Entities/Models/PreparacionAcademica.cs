namespace SitiosPersonal.Entities.Models
{
    public class PreparacionAcademica
    {
        public int id_preparacion { get; set; }

        public int id_oferente { get; set; }

        public int id_institucion { get; set; }

        public string titulo { get; set; }

        public DateTime fecha_inicio { get; set; }

        public DateTime fecha_fin { get; set; }
    }
}
