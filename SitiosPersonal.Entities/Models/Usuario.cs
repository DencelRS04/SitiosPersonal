namespace SitiosPersonal.Entities.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }

        public string usuario { get; set; }

        public string nombre_completo { get; set; }

        public string correo { get; set; }

        public string password_hash { get; set; }

        public string estado { get; set; }

        public int intentos_login { get; set; }
    }
}