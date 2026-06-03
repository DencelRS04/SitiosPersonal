using Microsoft.AspNetCore.Mvc;
using SitiosPersonal.Filters;
using SitiosPersonal.Repository.Data;
using SitiosPersonal.Repository.Repositories;
using SitiosPersonal.Services.Helpers;
using SitiosPersonal.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Login/Index", "");

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
});

builder.Services.AddSingleton<DbContext>();

builder.Services.AddScoped<LoginRepository>();
builder.Services.AddScoped<MenuRepository>();
builder.Services.AddScoped<BitacoraRepository>();
builder.Services.AddScoped<RolesRepository>();
builder.Services.AddScoped<PantallasRepository>();
builder.Services.AddScoped<UsuariosRepository>();
builder.Services.AddScoped<PermisosRepository>();

builder.Services.AddScoped<SessionPageFilter>();
builder.Services.AddScoped<PermisoPageFilter>();

builder.Services.AddScoped<BitacoraService>();
builder.Services.AddScoped<EncryptionHelper>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
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

app.MapRazorPages();

app.Run();
