namespace SitiosPersonal.Entities.Models
{
    public class Oferente
    {
        public int id_oferente { get; set; }

        public string identificacion { get; set; }

        public string tipo_identificacion { get; set; }

        public string nombre_completo { get; set; }

        public DateTime fecha_nacimiento { get; set; }

        public DateTime fecha_registro { get; set; }
    }
}
