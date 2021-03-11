using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Api.ActionFilters;
using Boilerplate.Commons.Attributes;
using Boilerplate.Commons.Static;
using Boilerplate.Commons.Validators;
using Boilerplate.Models;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Controllers.V1
{
    /// <summary>
    /// Uploads management
    /// </summary>
    [Authorize, ValidatesPermissions]
    [ApiVersion("1.0"), Route("api/v{version:apiVersion}/uploads")]
    public class UploadsController: BaseApiController
    {
        private readonly IUploadsService _uploadsService;

        /// <inheritdoc />
        public UploadsController(IUploadsService uploadsService)
        {
            _uploadsService = uploadsService;
        }

        /// <summary>
        /// Get file by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}"), PermissionRequired(Permissions.View)]
        public async Task<IActionResult> Get([ValidGuid] Guid id) => 
            await PreparePhysicalFileResponse(_uploadsService.Get, id);

        /// <summary>
        /// Upload files
        /// </summary>
        /// <returns></returns>
        [HttpPost, PermissionRequired(Permissions.Edit)]
        public async Task<BaseResponse<IEnumerable<Guid>>> Post(List<IFormFile> files) =>
            await PrepareResponse(_uploadsService.CreateUpload, files);
    }
}
