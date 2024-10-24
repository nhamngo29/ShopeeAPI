namespace Shopee.API.Installers
{
    public interface IInstaller
    {
        void InstrallServices(IServiceCollection services, IConfiguration configuration);
    }
}