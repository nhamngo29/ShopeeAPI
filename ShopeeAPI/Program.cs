﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shopee.API;
using Shopee.API.Extensions;
using Shopee.API.Middleware;
using Shopee.Application.Common;
using Shopee.Application.Common.Exceptions;
using System.Text;
using Serilog;
using Shopee.API.Installers;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
var configuration = builder.Configuration;
builder.Services.InstallerServiceInAssembly(configuration);
var app = builder.Build();
//app.ConfigureHealthCheck();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Use(async (context, next) =>
{
    try
    {

        await next.Invoke();
    }
    catch (UnauthorizedAccessException)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized");
    }
});
app.UseSwagger();
app.UseSwagger();
app.UseSwaggerUI(option =>
{
    option.InjectStylesheet("/swagger-ui/custom.css");
    option.SwaggerEndpoint("/swagger/OpenAPISpecification/swagger.json", "Clean Architecture Specification");
    option.RoutePrefix = "swagger";
});

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath,"wwwroot", "Images")),
    RequestPath = "/Resources"
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Nếu bạn sử dụng controller
});
app.Run();