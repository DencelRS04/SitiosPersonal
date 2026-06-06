using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class OferentesService : OferentesRepository
    {
        public OferentesService(DbContext context) : base(context)
        {
        }
    }
}
