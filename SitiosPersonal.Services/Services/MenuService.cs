using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class MenuService : MenuRepository
    {
        public MenuService(DbContext context) : base(context)
        {
        }
    }
}
