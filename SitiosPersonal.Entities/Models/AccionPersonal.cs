namespace SitiosPersonal.Entities.Models
{
    public class AccionPersonal
    {
        public int id_accion { get; set; }

        public string codigo { get; set; }

        public DateTime fecha_accion { get; set; }

        public string descripcion { get; set; }

        public int id_empleado { get; set; }

        public int id_empleado_jefatura { get; set; }
    }
}
