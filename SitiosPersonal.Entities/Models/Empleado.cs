namespace SitiosPersonal.Entities.Models
{
    public class Empleado
    {
        public int id_empleado { get; set; }

        public string numero_empleado { get; set; }

        public int id_oferente { get; set; }

        public int id_puesto { get; set; }

        public DateTime fecha_contratacion { get; set; }

        public string estado { get; set; }
    }
}
