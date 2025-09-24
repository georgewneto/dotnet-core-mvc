using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Models;
using Wpf.Ui.Controls;

namespace Projeto.WPF_UI
{
    public partial class EstadosWindow : FluentWindow
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
                MostrarStatus("Carregando estados...", InfoBarSeverity.Informational);
                var estados = await _estadoService.ObterTodosAsync();
                
                _estados.Clear();
                _estadosFiltrados.Clear();
                
                foreach (var estado in estados)
                {
                    _estados.Add(estado);
                    _estadosFiltrados.Add(estado);
                }
                
                MostrarStatus($"{_estados.Count} estado(s) carregado(s).", InfoBarSeverity.Success);
            }
            catch (Exception ex)
            {
                MostrarStatus($"Erro ao carregar estados: {ex.Message}", InfoBarSeverity.Error);
            }
        }

        private void ConfigurarBotoes(bool edicao = false)
        {
            btnNovo.IsEnabled = !edicao;
            btnSalvar.IsEnabled = edicao;
            btnEditar.IsEnabled = !edicao && _estadoSelecionado != null;
            btnExcluir.IsEnabled = !edicao && _estadoSelecionado != null;
            btnCancelar.IsEnabled = edicao;
            
            txtNome.IsEnabled = edicao;
            txtUF.IsEnabled = edicao;
            
            if (!edicao)
            {
                _modoEdicao = false;
                dgEstados.IsEnabled = true;
            }
        }

        private void HabilitarEdicao(bool novo = false)
        {
            _modoEdicao = true;
            dgEstados.IsEnabled = false;
            ConfigurarBotoes(true);
            
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
            dgEstados.SelectedItem = null;
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
                    ConfigurarBotoes();
                }
            }
            else
            {
                if (!_modoEdicao)
                {
                    _estadoSelecionado = null;
                    ConfigurarBotoes();
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
                    MostrarStatus("O campo Nome é obrigatório!", InfoBarSeverity.Warning);
                    txtNome.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUF.Text) || txtUF.Text.Length != 2)
                {
                    MostrarStatus("O campo UF é obrigatório e deve ter 2 caracteres!", InfoBarSeverity.Warning);
                    txtUF.Focus();
                    return;
                }

                MostrarStatus("Salvando...", InfoBarSeverity.Informational);

                if (_estadoSelecionado == null || txtId.Text == "") // Novo registro
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
                    
                    MostrarStatus("Estado adicionado com sucesso!", InfoBarSeverity.Success);
                }
                else // Edição
                {
                    _estadoSelecionado.Nome = txtNome.Text.Trim();
                    _estadoSelecionado.UF = txtUF.Text.Trim().ToUpper();

                    await _estadoService.AtualizarAsync(_estadoSelecionado);
                    
                    MostrarStatus("Estado atualizado com sucesso!", InfoBarSeverity.Success);
                }

                ConfigurarBotoes();
                dgEstados.Items.Refresh();
                LimparCampos();
            }
            catch (Exception ex)
            {
                MostrarStatus($"Erro ao salvar estado: {ex.Message}", InfoBarSeverity.Error);
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
            if (_estadoSelecionado == null) return;

            var result = System.Windows.MessageBox.Show($"Tem certeza que deseja excluir o estado '{_estadoSelecionado.Nome}'?", 
                                       "Confirmação de Exclusão", 
                                       System.Windows.MessageBoxButton.YesNo, 
                                       System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    MostrarStatus("Excluindo...", InfoBarSeverity.Informational);
                    await _estadoService.RemoverAsync(_estadoSelecionado.Id);
                    
                    _estados.Remove(_estadoSelecionado);
                    _estadosFiltrados.Remove(_estadoSelecionado);
                    
                    LimparCampos();
                    ConfigurarBotoes();
                    MostrarStatus("Estado excluído com sucesso!", InfoBarSeverity.Success);
                }
                catch (Exception ex)
                {
                    MostrarStatus($"Erro ao excluir estado: {ex.Message}", InfoBarSeverity.Error);
                }
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
            ConfigurarBotoes();
            MostrarStatus("Operação cancelada.", InfoBarSeverity.Informational);
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
            var termoBusca = txtBusca.Text.ToLower();
            _estadosFiltrados.Clear();

            var estadosFiltrados = string.IsNullOrWhiteSpace(termoBusca)
                ? _estados
                : _estados.Where(est => est.Nome.ToLower().Contains(termoBusca) || est.UF.ToLower().Contains(termoBusca));

            foreach (var estado in estadosFiltrados)
            {
                _estadosFiltrados.Add(estado);
            }
        }

        private bool FiltroAplicavel(Estado estado)
        {
            var termoBusca = txtBusca.Text.ToLower();
            if (string.IsNullOrWhiteSpace(termoBusca)) return true;
            return estado.Nome.ToLower().Contains(termoBusca) || estado.UF.ToLower().Contains(termoBusca);
        }

        private void MostrarStatus(string mensagem, InfoBarSeverity severidade)
        {
            infoBarStatus.Message = mensagem;
            infoBarStatus.Severity = severidade;
            infoBarStatus.IsOpen = true;
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}