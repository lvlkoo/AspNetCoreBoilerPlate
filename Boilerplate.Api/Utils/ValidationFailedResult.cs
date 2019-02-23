using System.Linq;
using Boilerplate.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Boilerplate.Api.Utils
{
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState) : base(PrepareResponse(modelState))
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity;
        }

        private static BaseResponse PrepareResponse(ModelStateDictionary modelState)
        {
            return new BaseResponse
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity,
                IsSuccess = false,
                Error = new ValidationResultModel
                {
                    Message = "Model validation filed",
                    Errors = modelState.Keys
                        .SelectMany(key => modelState[key].Errors.Select(x => new ValidationErrorModel
                        {
                            Field = key,
                            Message = x.ErrorMessage
                        }))
                        .ToList()
                }
            };
        }
    }
}
