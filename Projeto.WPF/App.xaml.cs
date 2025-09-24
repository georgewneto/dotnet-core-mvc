using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Projeto.Infraestrutura.Data;
using Projeto.Infraestrutura.Repositories;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Services;

namespace Projeto.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider? ServiceProvider { get; private set; }
    public IConfiguration? Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Configuração
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        // Configuração de Serviços
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();

        // Aplicar migrações automaticamente
        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }

        base.OnStartup(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configuração do SQLite
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(Configuration?.GetConnectionString("DefaultConnection")));

        // Registrar os repositórios
        services.AddScoped<IEstadoRepository, EstadoRepository>();
        services.AddScoped<ICidadeRepository, CidadeRepository>();

        // Registrar os serviços
        services.AddScoped<IEstadoService, EstadoService>();
        services.AddScoped<ICidadeService, CidadeService>();

        // Registrar as janelas
        services.AddTransient<MainWindow>();
        services.AddTransient<EstadosWindow>();
        services.AddTransient<CidadesWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        base.OnExit(e);
    }
}

