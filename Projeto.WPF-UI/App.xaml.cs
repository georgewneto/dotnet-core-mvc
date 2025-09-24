using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Projeto.Infraestrutura.Data;
using Projeto.Infraestrutura.Repositories;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Services;
using System;
using System.Windows;

namespace Projeto.WPF_UI;

public partial class App : Application
{
    public IServiceProvider? ServiceProvider { get; private set; }
    public IConfiguration? Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        using (var scope = ServiceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }

        var main = ServiceProvider.GetRequiredService<MainWindow>();
        main.Show();

        base.OnStartup(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(Configuration?.GetConnectionString("DefaultConnection")));

        services.AddScoped<IEstadoRepository, EstadoRepository>();
        services.AddScoped<ICidadeRepository, CidadeRepository>();
        services.AddScoped<IEstadoService, EstadoService>();
        services.AddScoped<ICidadeService, CidadeService>();

        services.AddTransient<MainWindow>();
        services.AddTransient<EstadosWindow>();
        services.AddTransient<CidadesWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (ServiceProvider is IDisposable d)
            d.Dispose();
        base.OnExit(e);
    }
}

