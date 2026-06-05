using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class PreparacionAcademicaService : PreparacionAcademicaRepository
    {
        public PreparacionAcademicaService(DbContext context) : base(context)
        {
        }
    }
}
