using Microsoft.AspNetCore.Diagnostics;
using Shopee.Application.Common.Exceptions;
using Shopee.Domain.Enums;
using System.Net;

namespace Shopee.API.Middleware;
public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {

        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            AllowStatusCode404Response = true,
            ExceptionHandler = async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorId = Guid.NewGuid();

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    string errorMessage = string.Empty;
                    string errorCode = string.Empty;

                    if (contextFeature.Error is UserFriendlyException userFriendlyException)
                    {
                        switch (userFriendlyException.ErrorCode)
                        {
                            case ErrorCode.NotFound:
                                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                errorMessage = userFriendlyException.UserFriendlyMessage;
                                break;
                            case ErrorCode.VersionConflict:
                                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                                errorMessage = userFriendlyException.UserFriendlyMessage;
                                break;
                            case ErrorCode.ItemAlreadyExists:
                                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                                errorMessage = userFriendlyException.UserFriendlyMessage;
                                break;
                            case ErrorCode.Conflict:
                                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                                errorMessage = userFriendlyException.UserFriendlyMessage;
                                break;
                            case ErrorCode.BadRequest:
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                errorMessage = userFriendlyException.UserFriendlyMessage;
                                break;
                            case ErrorCode.Unauthorized:
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                errorMessage = userFriendlyException.UserFriendlyMessage;
                                break;
                            case ErrorCode.Internal:
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                errorMessage = userFriendlyException.UserFriendlyMessage;
                                break;
                            case ErrorCode.UnprocessableEntity:
                                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                                errorMessage = userFriendlyException.UserFriendlyMessage;
                                break;
                            default:
                                context.Response.StatusCode = 500;
                                errorMessage = userFriendlyException.UserFriendlyMessage;
                                break;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 500;
                        errorMessage = "An error has occurred.";
                    }
                    await context.Response.WriteAsync($@"
                                {{
                                    ""errors"":[
                                        {{
                                            ""code"":""{errorCode}"",
                                            ""message"":""{errorMessage}, ErrorId:{errorId}""
                                        }}
                                    ]
                                }}");

                    //logger.LogError($"ErrorId:{errorId} Exception:{contextFeature.Error}");
                }
            }
        });
    }
}
