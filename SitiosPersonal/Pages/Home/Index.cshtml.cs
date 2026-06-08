using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SitiosPersonal.Pages.Home
{
    public class IndexModel : PageModel
    {
        public string? NombreCompleto { get; set; }

        public IActionResult OnGet()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                bool teniaSesion = Request.Cookies.ContainsKey("SesionIniciada");

                TempData["Mensaje"] = teniaSesion
                    ? "La sesión ha expirado. Por favor inicie sesión nuevamente."
                    : "Por favor inicie sesión para utilizar el sistema";

                return RedirectToPage("/Login/Index");
            }

            NombreCompleto = HttpContext.Session.GetString("NombreCompleto");

            return Page();
        }
    }
}
