using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Boilerplate.Api.ActionFilters
{
    public class PermissionAuthorizationHandler : AttributeAuthorizationHandler<IAuthorizationRequirement, AuthorizeWithPermissionsAttribute>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement, IEnumerable<AuthorizeWithPermissionsAttribute> attributes)
        {
            foreach (var permissionAttribute in attributes)
            {
                if (!await AuthorizeAsync(context.User, permissionAttribute.Permissions))
                {
                    return;
                }
            }

            context.Succeed(requirement);
        }

        private async Task<bool> AuthorizeAsync(ClaimsPrincipal user, string[] permission)
        {
            var userPermissions = user.FindAll("Permission");

            if (!permission.All(p => userPermissions.Any(c => c.Value == p)))
                return false;

            return true;
        }
    }
}
