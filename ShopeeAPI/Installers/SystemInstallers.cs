namespace Shopee.API.Installers
{
    public class SystemInstallers : IInstaller
    {
        public void InstrallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromDays(10);
                options.Cookie.HttpOnly = true;
            });
            services.AddEndpointsApiExplorer();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:3001", "http://localhost:3001", "http://localhost:3000", "https://localhost:3000", "http://localhost:8080", "https://localhost:8080", "http://localhost:8081", "https://localhost:8081") // Địa chỉ client
                            .AllowAnyMethod()
                            .AllowAnyHeader() // Cho phép bất kỳ header nào
                            .AllowCredentials(); // Cho phép gửi cookie
                    });
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}
