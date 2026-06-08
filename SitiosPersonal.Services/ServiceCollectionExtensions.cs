using Microsoft.Extensions.DependencyInjection;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Helpers;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSitiosPersonalServices(this IServiceCollection services)
        {
            services.AddSingleton<DbContext>();

            services.AddScoped<LoginRepository>();
            services.AddScoped<MenuRepository>();
            services.AddScoped<BitacoraRepository>();
            services.AddScoped<RolesRepository>();
            services.AddScoped<PantallasRepository>();
            services.AddScoped<UsuariosRepository>();
            services.AddScoped<PermisosRepository>();
            services.AddScoped<OferentesRepository>();
            services.AddScoped<EmpleadosRepository>();
            services.AddScoped<ConcursosRepository>();
            services.AddScoped<PreparacionAcademicaRepository>();
            services.AddScoped<ExperienciaLaboralRepository>();
            services.AddScoped<EntrevistasRepository>();
            services.AddScoped<AreasRepository>();
            services.AddScoped<PuestosRepository>();
            services.AddScoped<AccionesPersonalRepository>();
            services.AddScoped<RequisitosPuestoRepository>();

            services.AddScoped<LoginService>();
            services.AddScoped<MenuService>();
            services.AddScoped<BitacoraService>();
            services.AddScoped<BitacoraConsultaService>();
            services.AddScoped<RolesService>();
            services.AddScoped<PantallasService>();
            services.AddScoped<UsuariosService>();
            services.AddScoped<PermisosService>();
            services.AddScoped<OferentesService>();
            services.AddScoped<EmpleadosService>();
            services.AddScoped<ConcursosService>();
            services.AddScoped<PreparacionAcademicaService>();
            services.AddScoped<ExperienciaLaboralService>();
            services.AddScoped<EntrevistasService>();
            services.AddScoped<AreasService>();
            services.AddScoped<PuestosService>();
            services.AddScoped<AccionesPersonalService>();
            services.AddScoped<RequisitoPuestosService>();
            services.AddScoped<EncryptionHelper>();

            return services;
        }
    }
}
