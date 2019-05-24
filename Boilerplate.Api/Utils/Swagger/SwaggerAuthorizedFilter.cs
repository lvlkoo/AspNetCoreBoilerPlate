using System.Linq;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
                        Ref = "#/definitions/BaseResponse"
                    }
                });

                operation.Responses.Add("403", new Response
                {
                    Description = "Forbidden",
                    Schema = new Schema
                    {
                        Ref = "#/definitions/BaseResponse"
                    }
                });

                var filterDescriptor = context.ApiDescription.ActionDescriptor.FilterDescriptors.Where(_ =>
                    _.Filter.GetType() == typeof(AuthorizeFilter));

                var authFilters = filterDescriptor.Select(_ => _.Filter as AuthorizeFilter);
                var rolesRequirements = authFilters
                    .Where(_ => _?.Policy != null)
                    .SelectMany(_ => _.Policy.Requirements)
                    .OfType<RolesAuthorizationRequirement>()
                    .ToList();
                var roles = rolesRequirements.SelectMany(_ => _.AllowedRoles).ToList();
                if (roles.Any())
                    operation.Description += $"Roles allowed: {string.Join(", ", roles)}";
            }
        }
    }
}
