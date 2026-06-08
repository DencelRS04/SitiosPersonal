namespace SitiosPersonal.Entities.Models
{
    public class Concurso
    {
        public int id_concurso { get; set; }

        public string codigo { get; set; }

        public string nombre { get; set; }

        public DateTime fecha_inicio { get; set; }

        public DateTime fecha_fin { get; set; }

        public string estado { get; set; }
    }
}
