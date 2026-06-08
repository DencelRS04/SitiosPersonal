using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Entities.Models;

namespace SitiosPersonal.Services.Services
{
    public class UbicacionService : UbicacionRepository
    {
        public UbicacionService(DbContext context) : base(context) { }
    }
}
