using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Models;

namespace Projeto.WPF;

public partial class EstadosWindow : Window
{
    private readonly IEstadoService _estadoService;
    private ObservableCollection<Estado> _estados;
    private ObservableCollection<Estado> _estadosFiltrados;
    private Estado? _estadoSelecionado;
    private bool _modoEdicao = false;

    public EstadosWindow(IEstadoService estadoService)
    {
        InitializeComponent();
        _estadoService = estadoService;
        _estados = new ObservableCollection<Estado>();
        _estadosFiltrados = new ObservableCollection<Estado>();
        
        dgEstados.ItemsSource = _estadosFiltrados;
        
        ConfigurarBotoes();
        _ = CarregarEstadosAsync();
    }

    private async Task CarregarEstadosAsync()
    {
        try
        {
            lblStatus.Text = "Carregando estados...";
            var estados = await _estadoService.ObterTodosAsync();
            
            _estados.Clear();
            _estadosFiltrados.Clear();
            
            foreach (var estado in estados)
            {
                _estados.Add(estado);
                _estadosFiltrados.Add(estado);
            }
            
            lblStatus.Text = $"{_estados.Count} estado(s) carregado(s)";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao carregar estados: {ex.Message}", "Erro", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
            lblStatus.Text = "Erro ao carregar estados";
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
        txtUF.IsEnabled = false;
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
        txtUF.IsEnabled = true;
        
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
        txtUF.Text = "";
        _estadoSelecionado = null;
    }

    private void PreencherCampos(Estado estado)
    {
        txtId.Text = estado.Id.ToString();
        txtNome.Text = estado.Nome;
        txtUF.Text = estado.UF;
    }

    private void DgEstados_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgEstados.SelectedItem is Estado estadoSelecionado)
        {
            _estadoSelecionado = estadoSelecionado;
            PreencherCampos(estadoSelecionado);
            
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

            if (string.IsNullOrWhiteSpace(txtUF.Text) || txtUF.Text.Length != 2)
            {
                MessageBox.Show("O campo UF é obrigatório e deve ter 2 caracteres!", "Validação", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUF.Focus();
                return;
            }

            lblStatus.Text = "Salvando...";

            if (_estadoSelecionado == null) // Novo registro
            {
                var novoEstado = new Estado
                {
                    Nome = txtNome.Text.Trim(),
                    UF = txtUF.Text.Trim().ToUpper()
                };

                var estadoSalvo = await _estadoService.AdicionarAsync(novoEstado);
                _estados.Add(estadoSalvo);
                
                if (FiltroAplicavel(estadoSalvo))
                {
                    _estadosFiltrados.Add(estadoSalvo);
                }
                
                lblStatus.Text = "Estado adicionado com sucesso!";
            }
            else // Edição
            {
                _estadoSelecionado.Nome = txtNome.Text.Trim();
                _estadoSelecionado.UF = txtUF.Text.Trim().ToUpper();

                await _estadoService.AtualizarAsync(_estadoSelecionado);
                
                lblStatus.Text = "Estado atualizado com sucesso!";
            }

            ConfigurarBotoes();
            dgEstados.Items.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao salvar estado: {ex.Message}", "Erro", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
            lblStatus.Text = "Erro ao salvar estado";
        }
    }

    private void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_estadoSelecionado != null)
        {
            HabilitarEdicao();
        }
    }

    private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
    {
        if (_estadoSelecionado != null)
        {
            var resultado = MessageBox.Show(
                $"Deseja realmente excluir o estado '{_estadoSelecionado.Nome}'?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    lblStatus.Text = "Excluindo...";
                    await _estadoService.RemoverAsync(_estadoSelecionado.Id);
                    
                    _estados.Remove(_estadoSelecionado);
                    _estadosFiltrados.Remove(_estadoSelecionado);
                    
                    LimparCampos();
                    ConfigurarBotoes();
                    
                    lblStatus.Text = "Estado excluído com sucesso!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir estado: {ex.Message}", "Erro", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    lblStatus.Text = "Erro ao excluir estado";
                }
            }
        }
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        ConfigurarBotoes();
        
        if (_estadoSelecionado != null)
        {
            PreencherCampos(_estadoSelecionado);
        }
        else
        {
            LimparCampos();
        }
    }

    private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
    {
        FiltrarEstados();
    }

    private void BtnLimparBusca_Click(object sender, RoutedEventArgs e)
    {
        txtBusca.Text = "";
        FiltrarEstados();
    }

    private void FiltrarEstados()
    {
        var filtro = txtBusca.Text.ToLower();
        
        _estadosFiltrados.Clear();
        
        var estadosFiltrados = string.IsNullOrWhiteSpace(filtro) 
            ? _estados 
            : _estados.Where(e => e.Nome.ToLower().Contains(filtro) || 
                                 e.UF.ToLower().Contains(filtro));
        
        foreach (var estado in estadosFiltrados)
        {
            _estadosFiltrados.Add(estado);
        }
        
        lblStatus.Text = $"{_estadosFiltrados.Count} estado(s) exibido(s)";
    }

    private bool FiltroAplicavel(Estado estado)
    {
        var filtro = txtBusca.Text.ToLower();
        return string.IsNullOrWhiteSpace(filtro) || 
               estado.Nome.ToLower().Contains(filtro) || 
               estado.UF.ToLower().Contains(filtro);
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