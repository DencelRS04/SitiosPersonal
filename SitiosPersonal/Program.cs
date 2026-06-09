using Microsoft.AspNetCore.Mvc;
using SitiosPersonal.Filters;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    // No se agrega ruta manual para /Login porque Razor Pages
    // ya genera /Login automáticamente desde Pages/Login/Index.cshtml

    options.Conventions.AddPageRoute("/Puestos/Index", "Empleados/Puestos");
    options.Conventions.AddPageRoute("/Areas/Index", "Empleados/Areas");
    options.Conventions.AddPageRoute("/AccionesPersonal/Index", "Empleados/Acciones");

    options.Conventions.AddPageApplicationModelConvention("/Home/Index", model =>
    {
        model.Filters.Add(new ServiceFilterAttribute(typeof(SessionPageFilter)));
    });

    options.Conventions.AddFolderApplicationModelConvention("/Seguridad", model =>
    {
        model.Filters.Add(new ServiceFilterAttribute(typeof(PermisoPageFilter)));
    });

    options.Conventions.AddFolderApplicationModelConvention("/General", model =>
    {
        model.Filters.Add(new ServiceFilterAttribute(typeof(PermisoPageFilter)));
    });

    options.Conventions.AddFolderApplicationModelConvention("/Oferentes", model =>
    {
        model.Filters.Add(new ServiceFilterAttribute(typeof(PermisoPageFilter)));
    });

    options.Conventions.AddFolderApplicationModelConvention("/Concursos", model =>
    {
        model.Filters.Add(new ServiceFilterAttribute(typeof(PermisoPageFilter)));
    });

    options.Conventions.AddFolderApplicationModelConvention("/Entrevistas", model =>
    {
        model.Filters.Add(new ServiceFilterAttribute(typeof(PermisoPageFilter)));
    });
});

builder.Services.AddSitiosPersonalServices();

builder.Services.AddScoped<SessionPageFilter>();
builder.Services.AddScoped<PermisoPageFilter>();

// Leer SESION_MINUTOS desde la BD; si no existe, usar 5 como valor por defecto
int sessionMinutes = 5;
try
{
    using var tempProvider = builder.Services.BuildServiceProvider();
    var dbContext = tempProvider.GetRequiredService<DbContext>();
    var paramRepo = new ParametroRepository(dbContext);
    var param = paramRepo.ObtenerPorCodigo("SESION_MINUTOS");
    if (param != null && int.TryParse(param.valor, out int minFromDb) && minFromDb > 0)
        sessionMinutes = minFromDb;
}
catch { /* Si falla la lectura, se usa el valor por defecto */ }

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(sessionMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

// Cuando entren a la raíz del dominio, redirige al login.
// Ejemplo: https://tiusr27pl.cuc-carrera-ti.ac.cr/ -> /Login
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

// Ruta directa para cerrar sesión.
// Evita problemas con ?handler=Logout en Plesk.
app.MapGet("/Logout", context =>
{
    context.Session.Clear();
    context.Response.Cookies.Delete("SesionIniciada");

    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.MapRazorPages();

app.Run();