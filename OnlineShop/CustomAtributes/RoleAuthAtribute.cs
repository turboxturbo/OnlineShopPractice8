using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataBaseContext;

namespace OnlineShop.CustomAtributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthAtribute : Attribute, IAsyncActionFilter
    {
        private readonly int[] _roleid;
        public RoleAuthAtribute(int[] roleid)
        {
            _roleid = roleid;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dbcontext = context.HttpContext.RequestServices.GetRequiredService<ContextDb>();
            string token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new JsonResult(new {error = "Session is not given"}) { StatusCode = 401 };
                return;
            }
            var session = await dbcontext.Sessions.Include(u => u.user).FirstOrDefaultAsync(u => u.Token == token);
            if (session == null)
            {
                context.Result = new JsonResult(new { error = "Session is not found" }) { StatusCode = 401 };
                return;
            }
            if (!_roleid.Contains(session.user.IdRole))
            {
                context.Result = new JsonResult(new { error = "Not enough rights" }) { StatusCode = 403 };
                return;
            }
            await next();
        }
    }
}
