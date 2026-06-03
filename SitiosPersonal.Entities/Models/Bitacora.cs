namespace SitiosPersonal.Entities.Models
{
    public class Bitacora
    {
        public int id_bitacora { get; set; }

        public DateTime fecha { get; set; }

        public int? id_usuario { get; set; }

        public string usuario { get; set; }

        public string tipo { get; set; }

        public string entidad { get; set; }

        public string datos_anteriores { get; set; }

        public string datos_nuevos { get; set; }

        public string descripcion { get; set; }
    }
}