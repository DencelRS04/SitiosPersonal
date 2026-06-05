using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class EntrevistasService : EntrevistasRepository
    {
        public EntrevistasService(DbContext context) : base(context)
        {
        }
    }
}
