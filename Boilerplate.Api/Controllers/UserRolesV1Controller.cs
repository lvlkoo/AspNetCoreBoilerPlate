using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Api.ActionFilters;
using Boilerplate.Models;
using Boilerplate.Models.Auth;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Controllers
{
    [AuthorizeWithPermissions(Permission.UserPremission.Delete)]
    [ApiVersion("1.0"), Route("api/v{version:apiVersion}/user-roles")]
    public class UserRolesV1Controller: BaseApiController<IRolesService, RoleModel>
    {
        public UserRolesV1Controller(IRolesService service) : base(service)
        {
        }

        /// <summary>
        /// Get permissions list
        /// </summary>
        [HttpGet("permissions")]
        public virtual async Task<BaseResponse<IEnumerable<string>>> GetPermissions() =>
            await PrepareResponse(Service.GetPermissions);
    }
}
