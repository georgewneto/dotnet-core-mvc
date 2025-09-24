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
    public partial class CidadesWindow : FluentWindow
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
            cbEstado.ItemsSource = _estados;
            cbFiltroEstado.ItemsSource = _estados;

            ConfigurarBotoes();
            _ = CarregarDadosAsync();
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async Task CarregarDadosAsync()
        {
            try
            {
                MostrarStatus("Carregando dados...", InfoBarSeverity.Informational);

                var estados = await _estadoService.ObterTodosAsync();
                _estados.Clear();
                foreach (var est in estados.OrderBy(e => e.Nome))
                    _estados.Add(est);

                var cidades = await _cidadeService.ObterTodosAsync();
                _cidades.Clear();
                _cidadesFiltradas.Clear();
                foreach (var c in cidades)
                {
                    _cidades.Add(c);
                    _cidadesFiltradas.Add(c);
                }

                MostrarStatus($"{_cidades.Count} cidade(s) carregada(s).", InfoBarSeverity.Success);
            }
            catch (Exception ex)
            {
                MostrarStatus($"Erro ao carregar dados: {ex.Message}", InfoBarSeverity.Error);
            }
        }

        private void ConfigurarBotoes(bool edicao = false)
        {
            btnNovo.IsEnabled = !edicao;
            btnSalvar.IsEnabled = edicao;
            btnEditar.IsEnabled = !edicao && _cidadeSelecionada != null;
            btnExcluir.IsEnabled = !edicao && _cidadeSelecionada != null;
            btnCancelar.IsEnabled = edicao;

            txtNome.IsEnabled = edicao;
            cbEstado.IsEnabled = edicao;

            if (!edicao)
            {
                _modoEdicao = false;
                dgCidades.IsEnabled = true;
            }
        }

        private void HabilitarEdicao(bool novo = false)
        {
            _modoEdicao = true;
            dgCidades.IsEnabled = false;
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
            cbEstado.SelectedItem = null;
            _cidadeSelecionada = null;
            dgCidades.SelectedItem = null;
        }

        private void PreencherCampos(Cidade c)
        {
            txtId.Text = c.Id.ToString();
            txtNome.Text = c.Nome;
            if (c.EstadoId > 0)
            {
                var est = _estados.FirstOrDefault(e => e.Id == c.EstadoId);
                if (est != null)
                    cbEstado.SelectedItem = est;
                else
                {
                    cbEstado.SelectedValue = c.EstadoId;
                }
            }
        }

        private void DgCidades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCidades.SelectedItem is Cidade selecionada)
            {
                _cidadeSelecionada = selecionada;
                PreencherCampos(selecionada);
                if (!_modoEdicao)
                    ConfigurarBotoes();
            }
            else
            {
                if (!_modoEdicao)
                {
                    _cidadeSelecionada = null;
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

                if (cbEstado.SelectedItem is not Estado estadoSel)
                {
                    MostrarStatus("Selecione um Estado!", InfoBarSeverity.Warning);
                    cbEstado.Focus();
                    return;
                }

                MostrarStatus("Salvando...", InfoBarSeverity.Informational);

                if (_cidadeSelecionada == null || string.IsNullOrWhiteSpace(txtId.Text))
                {
                    var nova = new Cidade
                    {
                        Nome = txtNome.Text.Trim(),
                        EstadoId = estadoSel.Id
                    };
                    var salva = await _cidadeService.AdicionarAsync(nova);
                    salva.Estado = _estados.FirstOrDefault(e => e.Id == salva.EstadoId);
                    _cidades.Add(salva);
                    if (FiltroAplicavel(salva))
                        _cidadesFiltradas.Add(salva);

                    MostrarStatus("Cidade adicionada com sucesso!", InfoBarSeverity.Success);
                }
                else
                {
                    _cidadeSelecionada.Nome = txtNome.Text.Trim();
                    _cidadeSelecionada.EstadoId = estadoSel.Id;
                    await _cidadeService.AtualizarAsync(_cidadeSelecionada);
                    _cidadeSelecionada.Estado = _estados.FirstOrDefault(e => e.Id == _cidadeSelecionada.EstadoId);
                    MostrarStatus("Cidade atualizada com sucesso!", InfoBarSeverity.Success);
                }

                ConfigurarBotoes();
                dgCidades.Items.Refresh();
                LimparCampos();
            }
            catch (Exception ex)
            {
                MostrarStatus($"Erro ao salvar cidade: {ex.Message}", InfoBarSeverity.Error);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (_cidadeSelecionada != null)
                HabilitarEdicao();
        }

        private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (_cidadeSelecionada == null) return;

            var result = System.Windows.MessageBox.Show($"Tem certeza que deseja excluir a cidade '{_cidadeSelecionada.Nome}'?",
                                       "Confirmação de Exclusão",
                                       System.Windows.MessageBoxButton.YesNo,
                                       System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    MostrarStatus("Excluindo...", InfoBarSeverity.Informational);
                    await _cidadeService.RemoverAsync(_cidadeSelecionada.Id);
                    _cidades.Remove(_cidadeSelecionada);
                    _cidadesFiltradas.Remove(_cidadeSelecionada);
                    LimparCampos();
                    ConfigurarBotoes();
                    MostrarStatus("Cidade excluída com sucesso!", InfoBarSeverity.Success);
                }
                catch (Exception ex)
                {
                    MostrarStatus($"Erro ao excluir cidade: {ex.Message}", InfoBarSeverity.Error);
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
            FiltrarCidades();
        }

        private void CbFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FiltrarCidades();
        }

        private void BtnLimparBusca_Click(object sender, RoutedEventArgs e)
        {
            txtBusca.Text = string.Empty;
            cbFiltroEstado.SelectedItem = null;
            FiltrarCidades();
        }

        private void FiltrarCidades()
        {
            var termo = (txtBusca.Text ?? string.Empty).ToLower();
            var estadoFiltro = cbFiltroEstado.SelectedItem as Estado;

            _cidadesFiltradas.Clear();
            var query = _cidades.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(termo))
                query = query.Where(c => c.Nome.ToLower().Contains(termo));

            if (estadoFiltro != null)
                query = query.Where(c => c.EstadoId == estadoFiltro.Id);

            foreach (var c in query)
                _cidadesFiltradas.Add(c);
        }

        private bool FiltroAplicavel(Cidade c)
        {
            var termo = (txtBusca.Text ?? string.Empty).ToLower();
            var estadoFiltro = cbFiltroEstado.SelectedItem as Estado;

            if (!string.IsNullOrWhiteSpace(termo) && !c.Nome.ToLower().Contains(termo))
                return false;

            if (estadoFiltro != null && c.EstadoId != estadoFiltro.Id)
                return false;

            return true;
        }

        private void MostrarStatus(string mensagem, InfoBarSeverity severidade)
        {
            infoBarStatus.Message = mensagem;
            infoBarStatus.Severity = severidade;
            infoBarStatus.IsOpen = true;
        }
    }
}
