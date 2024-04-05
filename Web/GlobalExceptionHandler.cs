using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using YourApp.Shared.Common.Exceptions;
using FluentValidation;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace YourApp.API.Extensions
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            this.Errors = new Dictionary<string, List<string>>();
        }
        public Guid ErrorId { get; set; } = Guid.NewGuid();
        public int StatusCode { get; set; } = (int)HttpStatusCode.InternalServerError;
        public Uri RequestUri { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public DateTime UTCDateTime { get; set; } = DateTime.UtcNow;
        public string Message { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public static class GlobalExceptionHandler
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature == null) return;

                    var errorResponse = new ErrorResponse
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.GetBaseException().Message,
                        RequestUri = context.Request.GetUri()

                    };

                    switch (contextFeature.Error)
                    {
                        case ValidationException s:
                            var validationError = contextFeature.Error as ValidationException;
                            var errors = validationError.Errors.GroupBy(x => x.PropertyName).ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToList());
                            errorResponse.Errors = errors;
                            errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                            break;
                        case AccessDeniedException s:
                            errorResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                            break;
                        case NotFoundException s:
                            errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                        case BadRequestException s:
                            var badErrors = contextFeature.Error as BadRequestException;
                            errorResponse.Errors = badErrors.Errors;
                            errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                            break;
                        default:
                            break;
                    }

                    var responseJson = errorResponse.ToString();

                    if(errorResponse.StatusCode== (int)HttpStatusCode.InternalServerError)
                    {
                        var logger = loggerFactory.CreateLogger(nameof(GlobalExceptionHandler));
                        logger.LogError($"Something went wrong: {responseJson}");
                    }
                   
                    context.Response.StatusCode = errorResponse.StatusCode;
                    await context.Response.WriteAsync(responseJson);

                  
                });
            });
        }
    }
}
