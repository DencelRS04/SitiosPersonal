namespace SitiosPersonal.Entities.Models
{
    public class Entrevista
    {
        public int id_entrevista { get; set; }

        public int id_oferente { get; set; }

        public int id_empleado_entrevistador { get; set; }

        public DateTime fecha_entrevista { get; set; }

        public TimeSpan hora_entrevista { get; set; }

        public string estado { get; set; }

        public string observacion { get; set; }

        public DateTime fecha_creacion { get; set; }
    }
}
