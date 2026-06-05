using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class UsuariosService : UsuariosRepository
    {
        public UsuariosService(DbContext context) : base(context)
        {
        }
    }
}
