using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PaymentSystem.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            // ❌ NOT LOGGED IN
            if (string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // ❌ NOT ADMIN
            if (role != "Admin" && role != "SuperAdmin")
            {
                context.Result = new ViewResult
                {
                    ViewName = "AccessDenied"
                };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}