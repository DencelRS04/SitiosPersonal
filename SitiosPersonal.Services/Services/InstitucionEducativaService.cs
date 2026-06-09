using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class InstitucionEducativaService : InstitucionEducativaRepository
    {
        public InstitucionEducativaService(DbContext context) : base(context) { }
    }
}
