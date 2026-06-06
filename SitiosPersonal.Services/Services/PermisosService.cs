using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class PermisosService : PermisosRepository
    {
        public PermisosService(DbContext context) : base(context)
        {
        }
    }
}
