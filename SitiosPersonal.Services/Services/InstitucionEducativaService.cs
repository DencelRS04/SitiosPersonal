using System.Text.RegularExpressions;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Services.Services
{
    public class InstitucionEducativaService : InstitucionEducativaRepository
    {
        public InstitucionEducativaService(DbContext context) : base(context) { }

        public List<string> Validar(InstitucionEducativa institucion, int? idActual = null)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(institucion.codigo))
                errores.Add("El código es obligatorio.");
            else if (institucion.codigo.Length > 50)
                errores.Add("El código no puede superar los 50 caracteres.");

            if (string.IsNullOrWhiteSpace(institucion.nombre))
            {
                errores.Add("El nombre es obligatorio.");
            }
            else
            {
                if (institucion.nombre.Length > 150)
                    errores.Add("El nombre no puede superar los 150 caracteres.");
                if (!Regex.IsMatch(institucion.nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                    errores.Add("El nombre solo puede contener letras.");
            }

            if (!string.IsNullOrWhiteSpace(institucion.codigo) && ExisteCodigo(institucion.codigo, idActual))
                errores.Add("Ya existe una institución con ese código.");

            return errores;
        }
    }
}
