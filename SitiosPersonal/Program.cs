using Microsoft.AspNetCore.Mvc;
using SitiosPersonal.Filters;
using SitiosPersonal.Services;

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
