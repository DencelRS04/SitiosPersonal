using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SitiosPersonal.Filters
{
    public class SessionPageFilter : IAsyncPageFilter
    {
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public SessionPageFilter(ITempDataDictionaryFactory tempDataFactory)
        {
            _tempDataFactory = tempDataFactory;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            int? idUsuario = context.HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                var tempData = _tempDataFactory.GetTempData(context.HttpContext);

                bool teniaSesion = context.HttpContext.Request.Cookies.ContainsKey("SesionIniciada");

                tempData["Mensaje"] = teniaSesion
                    ? "La sesión ha expirado. Por favor inicie sesión nuevamente."
                    : "Por favor inicie sesión para utilizar el sistema";

                context.Result = new RedirectToPageResult("/Login/Index");
                return Task.CompletedTask;
            }

            return next();
        }
    }
}
