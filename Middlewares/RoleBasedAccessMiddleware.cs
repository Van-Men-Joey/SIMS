using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SIMS.Middlewares
{
    public class RoleBasedAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var role = context.Session.GetString("Role");

            // Chưa đăng nhập mà vào trang restricted
            if (string.IsNullOrEmpty(role))
            {
                if (path.StartsWith("/users") || path.StartsWith("/faculty") || path.StartsWith("/admin"))
                {
                    context.Response.Redirect("/Auth/Login?message=Please login to access this page");
                    return;
                }
            }

            // Student cố vào Admin hoặc Faculty
            if (role == "Student" && (path.StartsWith("/users") || path.StartsWith("/faculty")))
            {
                context.Response.Redirect("/AccessDenied");
                return;
            }

            // Faculty cố vào Admin
            if (role == "Faculty" && path.StartsWith("/users"))
            {
                context.Response.Redirect("/AccessDenied");
                return;
            }

            await _next(context);
        }
    }
}
