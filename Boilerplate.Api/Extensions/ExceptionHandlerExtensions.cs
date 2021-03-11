﻿using System.Net;
using Boilerplate.Commons.Exceptions;
using Boilerplate.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Boilerplate.Api.Extensions
{
    /// <summary>
    /// Extension handler configuration
    /// </summary>
    public static class ExceptionHandlerExtensions
    {
        /// <summary>
        /// Extension handler configuration
        /// </summary>
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger("ExceptionMiddleware");
                        logger.LogError(
                            $"App error: {contextFeature.Error}. Stack trace: {contextFeature.Error.StackTrace}");

                        var message = "Internal Server ErrorModel.";

                        if (contextFeature.Error is BaseApiException exception)
                        {
                            context.Response.StatusCode = exception.StatusCode;
                            message = exception.StatusMessage;
                        }

                        var error = new ErrorModel
                        {
                            Message = message
                        };

                        var response = new BaseResponse
                        {
                            StatusCode = context.Response.StatusCode,
                            IsSuccess = false,
                            Error = error
                        };

                        var serializerSettings = new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        };
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response, serializerSettings));
                    }
                });
            });
        }
    }
}