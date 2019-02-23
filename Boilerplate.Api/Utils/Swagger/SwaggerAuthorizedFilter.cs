using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Boilerplate.Api.Utils.Swagger
{
    public class SwaggerAuthorizedFilter: IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor.FilterDescriptors.Any(_ =>
                _.Filter.GetType() == typeof(AuthorizeFilter)))
            {
                operation.Responses.Add("401", new Response
                {
                    Description = "Unauthorized",
                    Schema = new Schema
                    {
                        Ref = "#/definitions/BaseResponse[ErrorModel]"
                    }
                });
            }
        }
    }
}
