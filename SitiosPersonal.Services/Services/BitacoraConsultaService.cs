using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class BitacoraConsultaService : BitacoraRepository
    {
        public BitacoraConsultaService(DbContext context) : base(context)
        {
        }
    }
}
