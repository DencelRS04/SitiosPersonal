namespace SitiosPersonal.Entities.Models
{
    public class Pantalla
    {
        public int id_pantalla { get; set; }

        public string nombre { get; set; }

        public string modulo { get; set; }

        public string ruta { get; set; }

        public string icono { get; set; }

        public int orden_menu { get; set; }

        public bool visible_menu { get; set; }

        public bool activo { get; set; }
    }
}
