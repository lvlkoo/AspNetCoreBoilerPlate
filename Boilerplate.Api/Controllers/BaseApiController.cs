using System;
using System.Threading.Tasks;
using Boilerplate.Models;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Controllers
{
    [ApiController, Produces("application/json")]
    public class BaseApiController: ControllerBase
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
            await dataResolver(arg);
            return new BaseResponse();
        }
        
        protected async Task<BaseResponse<TResult>> PrepareResponse<TRequest, TResult>(Func<TRequest, Task<TResult>> dataResolver, TRequest model)
        {
            return new BaseResponse<TResult>
            {
                Data = await dataResolver(model)
            };
        }

        protected async Task<BaseResponse<TResult>> PrepareResponse<TKey, TRequest, TResult>(Func<TKey, TRequest, Task<TResult>> dataResolver, TKey key, TRequest model)
        {
            return new BaseResponse<TResult>
            {
                Data = await dataResolver(key, model)
            };
        }
    }
}
