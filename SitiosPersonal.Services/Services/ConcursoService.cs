using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class ConcursosService : ConcursosRepository
    {
        public ConcursosService(DbContext context) : base(context)
        {
        }
    }
}
