using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Api.Utils.Swagger;
using Boilerplate.Models;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Controllers
{
    [Authorize("JWT")]
    [ApiVersion("1.0"), Route("api/v{version:apiVersion}/uploads")]
    public class UploadsV1Controller: BaseApiController
    {
        private readonly IUploadsService _uploadsService;

        public UploadsV1Controller(IUploadsService uploadsService)
        {
            _uploadsService = uploadsService;
        }

        /// <summary>
        /// Upload files
        /// </summary>
        /// <returns></returns>
        [HttpPost, SwaggerFileUpload]
        public async Task<BaseResponse<IEnumerable<Guid>>> Post() =>
            await PrepareResponse(_uploadsService.CreateUpload, Request.Form.Files);
    }
}
