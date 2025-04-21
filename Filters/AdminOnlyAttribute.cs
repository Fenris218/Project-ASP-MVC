using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace QuanLyPhongNet.Filters
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                // Chuyển hướng nếu không phải admin
                context.Result = new RedirectToActionResult("AccessDenied", "User", null);
            }
        }
    }
}
