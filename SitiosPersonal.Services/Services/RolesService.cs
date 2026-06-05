using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class RolesService : RolesRepository
    {
        public RolesService(DbContext context) : base(context)
        {
        }
    }
}
