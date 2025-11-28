using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PruebaTec.WEB.Filters
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string _requiredRole;

        public AuthorizeRoleAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("UsuarioRol");

            if (string.IsNullOrEmpty(role) || !string.Equals(role, _requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                var controller = (Controller)context.Controller;
                controller.TempData["AlertaTipo"] = "error";
                controller.TempData["AlertaMensaje"] = "Acceso denegado. No tienes permisos para ver esta sección.";
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
