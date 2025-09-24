using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Projeto.WPF_UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
{
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private void MenuEstados_Click(object sender, RoutedEventArgs e)
    {
        var estadosWindow = _serviceProvider.GetRequiredService<EstadosWindow>();
        estadosWindow.Show();
    }

    private void MenuCidades_Click(object sender, RoutedEventArgs e)
    {
        var cidadesWindow = _serviceProvider.GetRequiredService<CidadesWindow>();
        cidadesWindow.Show();
    }

    private void MenuSair_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}