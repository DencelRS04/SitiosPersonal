using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class LoginService : LoginRepository
    {
        public LoginService(DbContext context) : base(context)
        {
        }
    }
}
