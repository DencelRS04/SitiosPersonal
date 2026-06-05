using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class ExperienciaLaboralService : ExperienciaLaboralRepository
    {
        public ExperienciaLaboralService(DbContext context) : base(context)
        {
        }
    }
}
