using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Boilerplate.Api.Utils.Swagger
{
    public class SwaggerFileUploadFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
           if (context.MethodInfo.CustomAttributes.Any(_ => _.AttributeType == typeof(SwaggerFileUploadAttribute)))
               operation.Parameters.Add(new NonBodyParameter()
               {
                   Name = "file",
                   Type = "file",
                   In = "formData"
               });
        }
    }
}
