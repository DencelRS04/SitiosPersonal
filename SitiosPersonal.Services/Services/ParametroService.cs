using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Services.Services
{
    public class ParametroService : ParametroRepository
    {
        public ParametroService(DbContext context) : base(context) { }

        public List<string> Validar(Parametro parametro, int? idActual = null)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(parametro.codigo))
                errores.Add("El código es obligatorio.");
            else if (parametro.codigo.Length > 100)
                errores.Add("El código no puede superar los 100 caracteres.");

            if (string.IsNullOrWhiteSpace(parametro.valor))
                errores.Add("El valor es obligatorio.");
            else if (parametro.valor.Length > 500)
                errores.Add("El valor no puede superar los 500 caracteres.");

            if (!string.IsNullOrWhiteSpace(parametro.codigo) && ExisteCodigo(parametro.codigo, idActual))
                errores.Add("Ya existe un parámetro con ese código.");

            return errores;
        }
    }
}
