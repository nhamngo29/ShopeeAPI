using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shopee.API;
using Shopee.API.Extensions;
using Shopee.API.Middleware;
using Shopee.Application.Common;
using Shopee.Application.Common.Exceptions;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.Get<SettingConfiguration>()
    ?? throw ProgramException.AppsettingNotSetException();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
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
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("X-Access-Token"))
            {
                context.Token = context.Request.Cookies["X-Access-Token"];
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



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000", "https://localhost:3000", "http://localhost:8080", "https://localhost:8080", "http://localhost:8081","https://localhost:8081") // Địa chỉ client
                .AllowAnyMethod()
                .AllowAnyHeader() // Cho phép bất kỳ header nào
                .AllowCredentials(); // Cho phép gửi cookie
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//app.ConfigureHealthCheck();
app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.ConfigureExceptionHandler();
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

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Nếu bạn sử dụng controller
});
app.Run();