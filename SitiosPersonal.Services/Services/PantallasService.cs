using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class PantallasService : PantallasRepository
    {
        public PantallasService(DbContext context) : base(context)
        {
        }
    }
}
