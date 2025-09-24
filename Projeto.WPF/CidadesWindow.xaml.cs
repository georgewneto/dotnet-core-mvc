using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Models;

namespace Projeto.WPF;

public partial class CidadesWindow : Window
{
    private readonly ICidadeService _cidadeService;
    private readonly IEstadoService _estadoService;
    private ObservableCollection<Cidade> _cidades;
    private ObservableCollection<Cidade> _cidadesFiltradas;
    private ObservableCollection<Estado> _estados;
    private Cidade? _cidadeSelecionada;
    private bool _modoEdicao = false;

    public CidadesWindow(ICidadeService cidadeService, IEstadoService estadoService)
    {
        InitializeComponent();
        _cidadeService = cidadeService;
        _estadoService = estadoService;
        _cidades = new ObservableCollection<Cidade>();
        _cidadesFiltradas = new ObservableCollection<Cidade>();
        _estados = new ObservableCollection<Estado>();
        
        dgCidades.ItemsSource = _cidadesFiltradas;
        
        ConfigurarBotoes();
        _ = CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        await CarregarEstadosAsync();
        await CarregarCidadesAsync();
    }

    private async Task CarregarEstadosAsync()
    {
        try
        {
            var estados = await _estadoService.ObterTodosAsync();
            
            _estados.Clear();
            foreach (var estado in estados)
            {
                _estados.Add(estado);
            }
            
            // Configurar ComboBoxes
            cmbEstado.ItemsSource = _estados;
            
            // Adicionar opção "Todos" para filtro
            var estadosParaFiltro = new List<Estado> { new Estado { Id = 0, Nome = "Todos os Estados", UF = "" } };
            estadosParaFiltro.AddRange(_estados);
            cmbEstadoFiltro.ItemsSource = estadosParaFiltro;
            cmbEstadoFiltro.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao carregar estados: {ex.Message}", "Erro", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task CarregarCidadesAsync()
    {
        try
        {
            lblStatus.Text = "Carregando cidades...";
            var cidades = await _cidadeService.ObterTodosAsync();
            
            _cidades.Clear();
            _cidadesFiltradas.Clear();
            
            foreach (var cidade in cidades)
            {
                _cidades.Add(cidade);
                _cidadesFiltradas.Add(cidade);
            }
            
            lblStatus.Text = $"{_cidades.Count} cidade(s) carregada(s)";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao carregar cidades: {ex.Message}", "Erro", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
            lblStatus.Text = "Erro ao carregar cidades";
        }
    }

    private void ConfigurarBotoes()
    {
        btnNovo.IsEnabled = true;
        btnSalvar.IsEnabled = false;
        btnEditar.IsEnabled = false;
        btnExcluir.IsEnabled = false;
        btnCancelar.IsEnabled = false;
        
        txtNome.IsEnabled = false;
        cmbEstado.IsEnabled = false;
    }

    private void HabilitarEdicao(bool novo = false)
    {
        _modoEdicao = true;
        
        btnNovo.IsEnabled = false;
        btnSalvar.IsEnabled = true;
        btnEditar.IsEnabled = false;
        btnExcluir.IsEnabled = false;
        btnCancelar.IsEnabled = true;
        
        txtNome.IsEnabled = true;
        cmbEstado.IsEnabled = true;
        
        if (novo)
        {
            LimparCampos();
            txtNome.Focus();
        }
    }

    private void LimparCampos()
    {
        txtId.Text = "";
        txtNome.Text = "";
        cmbEstado.SelectedItem = null;
        _cidadeSelecionada = null;
    }

    private void PreencherCampos(Cidade cidade)
    {
        txtId.Text = cidade.Id.ToString();
        txtNome.Text = cidade.Nome;
        cmbEstado.SelectedValue = cidade.EstadoId;
    }

    private void DgCidades_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgCidades.SelectedItem is Cidade cidadeSelecionada)
        {
            _cidadeSelecionada = cidadeSelecionada;
            PreencherCampos(cidadeSelecionada);
            
            if (!_modoEdicao)
            {
                btnEditar.IsEnabled = true;
                btnExcluir.IsEnabled = true;
            }
        }
        else
        {
            if (!_modoEdicao)
            {
                btnEditar.IsEnabled = false;
                btnExcluir.IsEnabled = false;
                LimparCampos();
            }
        }
    }

    private void BtnNovo_Click(object sender, RoutedEventArgs e)
    {
        HabilitarEdicao(true);
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O campo Nome é obrigatório!", "Validação", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNome.Focus();
                return;
            }

            if (cmbEstado.SelectedValue == null)
            {
                MessageBox.Show("O campo Estado é obrigatório!", "Validação", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbEstado.Focus();
                return;
            }

            lblStatus.Text = "Salvando...";

            if (_cidadeSelecionada == null) // Novo registro
            {
                var novaCidade = new Cidade
                {
                    Nome = txtNome.Text.Trim(),
                    EstadoId = (int)cmbEstado.SelectedValue
                };

                var cidadeSalva = await _cidadeService.AdicionarAsync(novaCidade);
                _cidades.Add(cidadeSalva);
                
                if (FiltroAplicavel(cidadeSalva))
                {
                    _cidadesFiltradas.Add(cidadeSalva);
                }
                
                lblStatus.Text = "Cidade adicionada com sucesso!";
            }
            else // Edição
            {
                _cidadeSelecionada.Nome = txtNome.Text.Trim();
                _cidadeSelecionada.EstadoId = (int)cmbEstado.SelectedValue;

                await _cidadeService.AtualizarAsync(_cidadeSelecionada);
                
                lblStatus.Text = "Cidade atualizada com sucesso!";
            }

            ConfigurarBotoes();
            dgCidades.Items.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao salvar cidade: {ex.Message}", "Erro", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
            lblStatus.Text = "Erro ao salvar cidade";
        }
    }

    private void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_cidadeSelecionada != null)
        {
            HabilitarEdicao();
        }
    }

    private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
    {
        if (_cidadeSelecionada != null)
        {
            var resultado = MessageBox.Show(
                $"Deseja realmente excluir a cidade '{_cidadeSelecionada.Nome}'?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    lblStatus.Text = "Excluindo...";
                    await _cidadeService.RemoverAsync(_cidadeSelecionada.Id);
                    
                    _cidades.Remove(_cidadeSelecionada);
                    _cidadesFiltradas.Remove(_cidadeSelecionada);
                    
                    LimparCampos();
                    ConfigurarBotoes();
                    
                    lblStatus.Text = "Cidade excluída com sucesso!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir cidade: {ex.Message}", "Erro", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    lblStatus.Text = "Erro ao excluir cidade";
                }
            }
        }
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        ConfigurarBotoes();
        
        if (_cidadeSelecionada != null)
        {
            PreencherCampos(_cidadeSelecionada);
        }
        else
        {
            LimparCampos();
        }
    }

    private void CmbEstadoFiltro_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FiltrarCidades();
    }

    private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
    {
        FiltrarCidades();
    }

    private void BtnLimparBusca_Click(object sender, RoutedEventArgs e)
    {
        txtBusca.Text = "";
        FiltrarCidades();
    }

    private void BtnTodosEstados_Click(object sender, RoutedEventArgs e)
    {
        cmbEstadoFiltro.SelectedIndex = 0;
        FiltrarCidades();
    }

    private void FiltrarCidades()
    {
        var filtroTexto = txtBusca.Text.ToLower();
        var estadoSelecionado = cmbEstadoFiltro.SelectedValue;
        
        _cidadesFiltradas.Clear();
        
        var cidadesFiltradas = _cidades.AsEnumerable();
        
        // Filtro por texto
        if (!string.IsNullOrWhiteSpace(filtroTexto))
        {
            cidadesFiltradas = cidadesFiltradas.Where(c => 
                c.Nome.ToLower().Contains(filtroTexto) ||
                (c.Estado?.Nome.ToLower().Contains(filtroTexto) ?? false) ||
                (c.Estado?.UF.ToLower().Contains(filtroTexto) ?? false));
        }
        
        // Filtro por estado
        if (estadoSelecionado != null && (int)estadoSelecionado > 0)
        {
            cidadesFiltradas = cidadesFiltradas.Where(c => c.EstadoId == (int)estadoSelecionado);
        }
        
        foreach (var cidade in cidadesFiltradas)
        {
            _cidadesFiltradas.Add(cidade);
        }
        
        lblStatus.Text = $"{_cidadesFiltradas.Count} cidade(s) exibida(s)";
    }

    private bool FiltroAplicavel(Cidade cidade)
    {
        var filtroTexto = txtBusca.Text.ToLower();
        var estadoSelecionado = cmbEstadoFiltro.SelectedValue;
        
        bool passaFiltroTexto = string.IsNullOrWhiteSpace(filtroTexto) ||
                               cidade.Nome.ToLower().Contains(filtroTexto) ||
                               (cidade.Estado?.Nome.ToLower().Contains(filtroTexto) ?? false) ||
                               (cidade.Estado?.UF.ToLower().Contains(filtroTexto) ?? false);
        
        bool passaFiltroEstado = estadoSelecionado == null || 
                                (int)estadoSelecionado == 0 || 
                                cidade.EstadoId == (int)estadoSelecionado;
        
        return passaFiltroTexto && passaFiltroEstado;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (_modoEdicao)
        {
            var resultado = MessageBox.Show(
                "Existem alterações não salvas. Deseja realmente fechar?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        
        base.OnClosing(e);
    }
}