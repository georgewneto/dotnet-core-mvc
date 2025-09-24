using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace Projeto.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MenuItem_Estados_Click(object sender, RoutedEventArgs e)
    {
        AbrirEstados();
    }

    private void MenuItem_Cidades_Click(object sender, RoutedEventArgs e)
    {
        AbrirCidades();
    }

    private void MenuItem_Sair_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void BtnEstados_Click(object sender, RoutedEventArgs e)
    {
        AbrirEstados();
    }

    private void BtnCidades_Click(object sender, RoutedEventArgs e)
    {
        AbrirCidades();
    }

    private void AbrirEstados()
    {
        var app = (App)Application.Current;
        var estadosWindow = app.ServiceProvider?.GetService<EstadosWindow>();
        if (estadosWindow != null)
        {
            estadosWindow.Show();
        }
    }

    private void AbrirCidades()
    {
        var app = (App)Application.Current;
        var cidadesWindow = app.ServiceProvider?.GetService<CidadesWindow>();
        if (cidadesWindow != null)
        {
            cidadesWindow.Show();
        }
    }
}