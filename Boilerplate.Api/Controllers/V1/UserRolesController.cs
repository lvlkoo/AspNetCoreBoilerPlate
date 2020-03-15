using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Api.ActionFilters;
using Boilerplate.Commons.Attributes;
using Boilerplate.Commons.Static;
using Boilerplate.Models;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Controllers.V1
{
    [Authorize, ValidatesPermissions]
    [ApiVersion("1.0"), Route("api/v{version:apiVersion}/user-roles")]
    public class UserRolesController: BaseApiController<IRolesService, RoleModel>
    {
        public UserRolesController(IRolesService service) : base(service)
        {
        }

        /// <summary>
        /// Get permissions list
        /// </summary>
        [HttpGet("permissions"), PermissionRequired(Permissions.View)]
        public virtual async Task<BaseResponse<IEnumerable<string>>> GetPermissions() =>
            await PrepareResponse(Service.GetPermissions);
    }
}
