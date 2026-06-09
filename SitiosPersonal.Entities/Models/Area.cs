namespace SitiosPersonal.Entities.Models
{
    public class Area
    {
        public int id_area { get; set; }

        public string codigo { get; set; }

        public string nombre { get; set; }

        public int? id_empleado_jefatura { get; set; }
    }
}
