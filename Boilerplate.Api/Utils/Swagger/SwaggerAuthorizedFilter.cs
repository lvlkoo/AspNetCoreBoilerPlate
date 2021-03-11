using System.Collections.Generic;
using System.Linq;
using Boilerplate.Api.ActionFilters;
using Boilerplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Boilerplate.Api.Utils.Swagger
{
    /// <summary>
    /// Provide auth errors metadata
    /// </summary>
    public class SwaggerAuthorizedFilter : IOperationFilter
    {
        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor.EndpointMetadata.Any(_ =>
                _.GetType() == typeof(AuthorizeAttribute)))
            {
                var baseResponseContent = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/json", new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = typeof(BaseResponse).Name,
                                    Type = ReferenceType.Schema
                                }
                            }
                        }
                    }
                };

                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = "Unauthorized",
                    Content = baseResponseContent
                });

                operation.Responses.Add("403", new OpenApiResponse
                {
                    Description = "Forbidden",
                    Content = baseResponseContent
                });

                var filterDescriptor = context.ApiDescription.ActionDescriptor.FilterDescriptors.Where(_ =>
                    _.Filter.GetType() == typeof(PermissionRequiredAttribute));

                var authFilters = filterDescriptor.Select(_ => _.Filter as PermissionRequiredAttribute);
                var permissionsRequired = authFilters
                    .SelectMany(_ => _?.Permissions)
                    .ToList();

                if (permissionsRequired.Any())
                    operation.Description += $"Permissions required: {string.Join(", ", permissionsRequired)}";
            }
        }
    }
}