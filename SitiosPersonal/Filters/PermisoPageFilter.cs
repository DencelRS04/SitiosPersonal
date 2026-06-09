using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SitiosPersonal.Services.Services;

namespace SitiosPersonal.Filters
{
    public class PermisoPageFilter : IAsyncPageFilter
    {
        private readonly PermisosService _repository;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public PermisoPageFilter(
            PermisosService repository,
            ITempDataDictionaryFactory tempDataFactory)
        {
            _repository = repository;
            _tempDataFactory = tempDataFactory;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(
            PageHandlerExecutingContext context,
            PageHandlerExecutionDelegate next)
        {
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";

            int? idUsuario = context.HttpContext.Session.GetInt32("IdUsuario");
            var tempData = _tempDataFactory.GetTempData(context.HttpContext);

            if (idUsuario == null)
            {
                bool teniaSesion = context.HttpContext.Request.Cookies.ContainsKey("SesionIniciada");

                tempData["Mensaje"] = teniaSesion
                    ? "La sesión ha expirado. Por favor inicie sesión nuevamente."
                    : "Por favor inicie sesión para utilizar el sistema";

                context.Result = new RedirectToPageResult("/Login/Index");
                return;
            }

            string rutaActual = context.HttpContext.Request.Path.Value ?? string.Empty;

            if (_repository.EsAdministrador(idUsuario.Value))
            {
                await next();
                return;
            }

            var rutasPermitidas = _repository.ObtenerRutasPermitidas(idUsuario.Value);

            bool tienePermiso = rutasPermitidas.Any(ruta =>
                !string.IsNullOrWhiteSpace(ruta)
                && rutaActual.StartsWith(ruta, StringComparison.OrdinalIgnoreCase)
            );

            if (!tienePermiso)
            {
                tempData["Error"] = "No tiene permisos para acceder a esta pantalla.";
                context.Result = new RedirectToPageResult("/Home/Index");
                return;
            }

            await next();
        }
    }
}