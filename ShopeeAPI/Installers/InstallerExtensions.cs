namespace Shopee.API.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallerServiceInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var installer = typeof(Program).Assembly.ExportedTypes.Where(t => typeof(IInstaller).IsAssignableFrom(t) && !t.IsInterface
            && !t.IsAbstract).Select(Activator.CreateInstance).Cast<IInstaller>().ToList();
            installer.ForEach(installer => installer.InstrallServices(services, configuration));
        }
    }
}