using Newtonsoft.Json;
using Shopee.Application.Common.Exceptions;
using System.Net;

namespace Shopee.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BadRequestException ex)
            {
                await HandleBadRequestExceptionAsync(httpContext, ex);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(httpContext, ex);
            }
            catch (ConflictException ex)
            {
                await HandleConflictExceptionAsync(httpContext, ex);
            }
            catch (HttpStatusException ex)
            {
                await HandleHttpStatusException(httpContext, ex);
            }
            catch (NotFoundException ex)
            {
                await HandleNotFuoundExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private Task HandleNotFuoundExceptionAsync(HttpContext context, NotFoundException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errorResponse = new
            {
                isSuccess = false,
                message = exception.Message,
                statusCode = 404,
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
        private Task HandleBadRequestExceptionAsync(HttpContext context, BadRequestException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errorResponse = new
            {
                isSuccess = false,
                message = exception.Message,
                statusCode = 400,
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }

        private Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 422;

            var errorResponse = new
            {
                message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu",
                response = exception.Errors
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new
            {
                Message = "Lỗi hệ thống",
            };
           
            _logger.LogError(exception,$"{exception.Message} metho {_next.Method.Name}",new{statucCode=HttpStatusCode.InternalServerError });
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }

        private Task HandleConflictExceptionAsync(HttpContext context, ConflictException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            var errorResponse = new
            {
                isSuccess = false,
                message = exception.Message,
                statusCode = 409,
                response = exception.Errors
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }

        public Task HandleHttpStatusException(HttpContext context, HttpStatusException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.StatusCode;
            var errorResponse = new
            {
                isSuccess = false,
                message = exception.Message,
                statusCode = exception.StatusCode
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}