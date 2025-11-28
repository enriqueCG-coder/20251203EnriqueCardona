using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PruebaTec.WEB.Filters
{
    public class AuthorizeTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                var controller = (Controller)context.Controller;
                controller.TempData["AlertaTipo"] = "error";
                controller.TempData["AlertaMensaje"] = "Acceso denegado. Inicia sesión.";
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
        }
    }
}
