using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Services.Services
{
    public class CompaniaService : CompaniaRepository
    {
        public CompaniaService(DbContext context) : base(context) { }

        public List<string> Validar(Compania compania, int? idActual = null)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(compania.codigo))
                errores.Add("El código es obligatorio.");
            else if (compania.codigo.Length > 50)
                errores.Add("El código no puede superar los 50 caracteres.");

            if (string.IsNullOrWhiteSpace(compania.nombre))
                errores.Add("El nombre es obligatorio.");
            else if (compania.nombre.Length > 150)
                errores.Add("El nombre no puede superar los 150 caracteres.");

            if (!string.IsNullOrWhiteSpace(compania.codigo) && ExisteCodigo(compania.codigo, idActual))
                errores.Add("Ya existe una compañía con ese código.");

            return errores;
        }
    }
}
