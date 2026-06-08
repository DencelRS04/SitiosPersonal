using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class CompaniaService : CompaniaRepository
    {
        public CompaniaService(DbContext context) : base(context) { }
    }
}
