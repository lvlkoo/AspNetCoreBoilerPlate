using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Api.ActionFilters;
using Boilerplate.Api.Utils;
using Boilerplate.Commons.Exceptions;
using Boilerplate.Commons.Static;
using Boilerplate.Commons.Validators;
using Boilerplate.Entities;
using Boilerplate.Models;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Controllers
{
    [ApiController, Produces("application/json")]
    public class BaseApiController : ControllerBase
    {
        protected async Task<BaseResponse<TResult>> PrepareResponse<TResult>(Func<Task<TResult>> dataResolver)
        {
            return new BaseResponse<TResult>
            {
                Data = await dataResolver()
            };
        }

        protected async Task<BaseResponse> PrepareResponse<TRequest>(Func<TRequest, Task> dataResolver, TRequest arg)
        {
            if (arg == null)
                throw new BadRequestException();

            await dataResolver(arg);
            return new BaseResponse();
        }

        protected async Task<BaseResponse<TResult>> PrepareResponse<TRequest, TResult>(Func<TRequest, Task<TResult>> dataResolver, TRequest model)
        {
            if (model == null)
                throw new BadRequestException();

            return new BaseResponse<TResult>
            {
                Data = await dataResolver(model)
            };
        }

        protected async Task<BaseResponse<TResult>> PrepareResponse<TKey, TRequest, TResult>(Func<TKey, TRequest, Task<TResult>> dataResolver, TKey key, TRequest model)
        {
            if (model == null)
                throw new BadRequestException();

            return new BaseResponse<TResult>
            {
                Data = await dataResolver(key, model)
            };
        }

        protected async Task<BaseResponse<TResult>> PrepareResponse<TKey, TRequest, TResult>(Func<TKey, TKey, TRequest, Task<TResult>> dataResolver, TKey key, TKey key2, TRequest model)
        {
            if (model == null)
                throw new BadRequestException();

            return new BaseResponse<TResult>
            {
                Data = await dataResolver(key, key2, model)
            };
        }

        protected async Task<BaseResponse> PrepareResponse<TKey, TRequest>(Func<TKey, TRequest, Task> dataResolver, TKey key, TRequest model)
        {
            if (model == null)
                throw new BadRequestException();

            await dataResolver(key, model);
            return new BaseResponse();
        }

        protected async Task<IActionResult> PreparePhysicalFileResponse<TKey>(Func<TKey, Task<FileUpload>> dataResolver, TKey key)
        {
            var upload = await dataResolver(key);
            return PhysicalFile(upload.FilePath, upload.ContentType);
        }
    }

    public class BaseApiController<TService, TModel> : BaseApiController where TModel : IModel where TService : ICrudService<TModel>
    {
        protected readonly TService Service;

        public BaseApiController(TService service)
        {
            Service = service;
        }

        /// <summary>
        /// Get list
        /// </summary>
        [HttpGet, PermissionRequired(Permissions.View)]
        public virtual async Task<BaseResponse<IEnumerable<TModel>>> Get() =>
            await PrepareResponse(Service.Get);

        /// <summary>
        /// Get by id
        /// </summary>
        [HttpGet("{id}"), PermissionRequired(Permissions.View)]
        [ProducesErrorResponse(StatusCodes.Status404NotFound)]
        public virtual async Task<BaseResponse<TModel>> Get([ValidGuid] Guid id) =>
            await PrepareResponse(Service.Get, id);

        /// <summary>
        /// Create
        /// </summary>
        [HttpPost, PermissionRequired(Permissions.Edit)]
        public virtual async Task<BaseResponse<TModel>> Post(TModel model) =>
            await PrepareResponse(Service.Create, model);

        /// <summary>
        /// Update
        /// </summary>
        [HttpPut("{id}"), PermissionRequired(Permissions.Edit)]
        [ProducesErrorResponse(StatusCodes.Status404NotFound)]
        public virtual async Task<BaseResponse<TModel>> Put([ValidGuid] Guid id, TModel model) =>
            await PrepareResponse(Service.Update, id, model);

        /// <summary>
        /// Delete
        /// </summary>
        [HttpDelete("{id}"), PermissionRequired(Permissions.Delete)]
        [ProducesErrorResponse(StatusCodes.Status404NotFound)]
        public virtual async Task<BaseResponse> Delete([ValidGuid] Guid id) =>
            await PrepareResponse(Service.Delete, id);
    }
}
