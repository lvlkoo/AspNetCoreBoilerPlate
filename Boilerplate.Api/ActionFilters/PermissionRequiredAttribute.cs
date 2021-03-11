using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Boilerplate.Api.ActionFilters
{
    /// <summary>
    /// Provide permissions check
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionRequiredAttribute : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// Required permissions
        /// </summary>
        public string[] Permissions { get; }

        /// <inheritdoc />
        public PermissionRequiredAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }

        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            var permissions = user.FindAll("Permission").Select(c => c.Value).ToList();
            var controller = ((ControllerActionDescriptor) context.ActionDescriptor).ControllerName;
            var permissionsRequired = Permissions.Select(p => $"{controller}/{p}").ToList();
            if (permissionsRequired.All(p => permissions.Contains(p)))
            {
                await next();
            }
            else
            {
                context.Result = new ForbidResult(JwtBearerDefaults.AuthenticationScheme);
            }
        }
    }
}
