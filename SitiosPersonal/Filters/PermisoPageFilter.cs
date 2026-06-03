using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Filters
{
    public class PermisoPageFilter : IAsyncPageFilter
    {
        private readonly PermisosRepository _repository;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public PermisoPageFilter(
            PermisosRepository repository,
            ITempDataDictionaryFactory tempDataFactory)
        {
            _repository = repository;
            _tempDataFactory = tempDataFactory;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            int? idUsuario = context.HttpContext.Session.GetInt32("IdUsuario");
            var tempData = _tempDataFactory.GetTempData(context.HttpContext);

            if (idUsuario == null)
            {
                bool teniaSesion = context.HttpContext.Request.Cookies.ContainsKey("SesionIniciada");

                tempData["Mensaje"] = teniaSesion
                    ? "La sesión ha expirado. Por favor inicie sesión nuevamente."
                    : "Por favor inicie sesión para utilizar el sistema";

                context.Result = new RedirectToPageResult("/Login/Index");
                return Task.CompletedTask;
            }

            string rutaActual = context.HttpContext.Request.Path.Value ?? string.Empty;

            var rutasPermitidas = _repository.ObtenerRutasPermitidas(idUsuario.Value);

            bool tienePermiso = rutasPermitidas.Any(ruta =>
                !string.IsNullOrWhiteSpace(ruta)
                && rutaActual.StartsWith(ruta, StringComparison.OrdinalIgnoreCase)
            );

            if (!tienePermiso)
            {
                tempData["Error"] = "No tiene permisos para acceder a esta pantalla.";
                context.Result = new RedirectToPageResult("/Home/Index");
                return Task.CompletedTask;
            }

            return next();
        }
    }
}
