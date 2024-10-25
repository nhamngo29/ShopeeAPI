using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Shopee.API;
using Shopee.API.Extensions;
using Shopee.API.Middleware;
using Shopee.Application.Common;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.Common.Interfaces;
using Shopee.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.Get<SettingConfiguration>()
    ?? throw ProgramException.AppsettingNotSetException();

builder.Services.AddSingleton(configuration);
builder.Services.AddWebAPIService(configuration);
// For authentication

builder.Services.AddAppDI(builder.Configuration);

// Configuration for token
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new { message = "Token đã hết hạn." });
                return context.Response.WriteAsync(result);
            }
            return Task.CompletedTask;
        }
    };
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = configuration.Jwt.Audience,
        ValidIssuer = configuration.Jwt.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Jwt.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

// Dependency injection with key
builder.Services.AddSingleton<ITokenService,TokenService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader() // Allow any header
            .AllowCredentials();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.ConfigureHealthCheck();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.ConfigureExceptionHandler();
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
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(option =>
    {
        option.InjectStylesheet("/swagger-ui/custom.css");
        option.SwaggerEndpoint("/swagger/OpenAPISpecification/swagger.json", "Clean Architecture Specification");
        option.RoutePrefix = "swagger";
    });

}
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Nếu bạn sử dụng controller
});
app.Run();