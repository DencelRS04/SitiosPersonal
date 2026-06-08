using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class ParametroService : ParametroRepository
    {
        public ParametroService(DbContext context) : base(context) { }
    }
}
