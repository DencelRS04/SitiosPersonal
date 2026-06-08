namespace SitiosPersonal.Entities.Models
{
    public class Puesto
    {
        public int id_puesto { get; set; }

        public string codigo { get; set; }

        public string nombre { get; set; }

        public decimal monto_salario { get; set; }

        public int? id_puesto_jefatura { get; set; }

        public bool activo { get; set; }
    }
}
