# Projeto.WPF - Sistema de Cadastro de Estados e Cidades

## 📋 Descrição

Este é um projeto WPF (Windows Presentation Foundation) desenvolvido em .NET 9 que implementa um sistema completo de cadastro de Estados e Cidades. O projeto faz parte de uma solução maior e reutiliza a arquitetura existente de services e repositories.

## 🏗️ Arquitetura

O projeto segue os princípios da **Arquitetura Limpa** e **DDD (Domain-Driven Design)**, reutilizando as camadas já existentes na solução:

```
┌─────────────────────────────────────┐
│           Projeto.WPF               │
│        (Apresentação WPF)           │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│      Projeto.RegrasDeNegocio        │
│    (Services + Models + Interfaces) │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│      Projeto.Infraestrutura         │
│   (Repositories + DbContext + EF)   │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│          SQLite Database            │
│            (app.db)                 │
└─────────────────────────────────────┘
```

### Dependências entre Projetos

- **Projeto.WPF** → **Projeto.RegrasDeNegocio** (Services e Models)
- **Projeto.WPF** → **Projeto.Infraestrutura** (Entity Framework e Repositories)
- **Projeto.RegrasDeNegocio** → **Projeto.Infraestrutura** (Implementações dos Repositories)

## 🚀 Comandos de Criação

### 1. Criação do Projeto WPF

```bash
# Navegar para o diretório da solução
cd c:\developer\dotNet\dotnet-core-mvc

# Criar projeto WPF
dotnet new wpf -n Projeto.WPF
```

### 2. Adição de Referências

```bash
# Navegar para o projeto WPF
cd Projeto.WPF

# Adicionar referência aos projetos da solução
dotnet add reference ..\Projeto.RegrasDeNegocio\Projeto.RegrasDeNegocio.csproj
dotnet add reference ..\Projeto.Infraestrutura\Projeto.Infraestrutura.csproj
```

### 3. Instalação de Pacotes NuGet

```bash
# Injeção de Dependências
dotnet add package Microsoft.Extensions.DependencyInjection

# Configuração
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
```

### 4. Adição à Solução

```bash
# Voltar ao diretório raiz
cd ..

# Adicionar projeto à solução
dotnet sln WebApplicationMvc.sln add .\Projeto.WPF\Projeto.WPF.csproj
```

### 5. Build e Execução

```bash
# Compilar o projeto
dotnet build .\Projeto.WPF\Projeto.WPF.csproj

# Compilar toda a solução
dotnet build WebApplicationMvc.sln

# Executar o projeto WPF
dotnet run --project .\Projeto.WPF\Projeto.WPF.csproj
```

## 📁 Estrutura de Arquivos

```
Projeto.WPF/
├── 📄 README.md                    # Esta documentação
├── 📄 Projeto.WPF.csproj          # Arquivo de projeto
├── 📄 appsettings.json            # Configurações (connection string)
├── 📄 App.xaml                    # Definições globais da aplicação
├── 📄 App.xaml.cs                 # Configuração de DI e inicialização
├── 📄 MainWindow.xaml             # Tela principal (XAML)
├── 📄 MainWindow.xaml.cs          # Code-behind da tela principal
├── 📄 EstadosWindow.xaml          # Tela de gestão de Estados (XAML)
├── 📄 EstadosWindow.xaml.cs       # Code-behind dos Estados
├── 📄 CidadesWindow.xaml          # Tela de gestão de Cidades (XAML)
└── 📄 CidadesWindow.xaml.cs       # Code-behind das Cidades
```

## ⚙️ Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=..\\Projeto.Web\\app.db"
  }
}
```

O banco de dados SQLite é compartilhado com o projeto Web, garantindo consistência de dados.

### Injeção de Dependências (App.xaml.cs)

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // Entity Framework
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(Configuration?.GetConnectionString("DefaultConnection")));

    // Repositories
    services.AddScoped<IEstadoRepository, EstadoRepository>();
    services.AddScoped<ICidadeRepository, CidadeRepository>();

    // Services
    services.AddScoped<IEstadoService, EstadoService>();
    services.AddScoped<ICidadeService, CidadeService>();

    // Windows
    services.AddTransient<MainWindow>();
    services.AddTransient<EstadosWindow>();
    services.AddTransient<CidadesWindow>();
}
```

## 🖼️ Interface do Usuário

### 1. Tela Principal (MainWindow)

**Características:**
- Design centralizado e elegante
- Menu superior para navegação
- Botões de acesso rápido
- Status bar informativo

**Funcionalidades:**
- Navegação via menu ou botões
- Acesso às telas de Estados e Cidades
- Opção de sair da aplicação

### 2. Tela de Estados (EstadosWindow)

**Layout:**
- **Lado Esquerdo:** DataGrid com lista de estados
- **Lado Direito:** Formulário para CRUD
- **Topo:** Barra de busca/filtro
- **Rodapé:** Barra de status

**Funcionalidades:**
- ✅ **Create:** Adicionar novos estados
- ✅ **Read:** Listar e visualizar estados
- ✅ **Update:** Editar estados existentes
- ✅ **Delete:** Remover estados (com confirmação)
- 🔍 **Busca:** Filtro por nome ou UF
- ✋ **Validação:** Campos obrigatórios e tamanhos máximos

**Campos do Formulário:**
- **ID:** Somente leitura (auto-incremento)
- **Nome:** Texto até 50 caracteres (obrigatório)
- **UF:** Texto de 2 caracteres maiúsculos (obrigatório)

### 3. Tela de Cidades (CidadesWindow)

**Layout:**
- **Lado Esquerdo:** DataGrid com lista de cidades
- **Lado Direito:** Formulário para CRUD
- **Topo:** Filtros por estado e busca de texto
- **Rodapé:** Barra de status

**Funcionalidades:**
- ✅ **Create:** Adicionar novas cidades
- ✅ **Read:** Listar e visualizar cidades
- ✅ **Update:** Editar cidades existentes
- ✅ **Delete:** Remover cidades (com confirmação)
- 🔍 **Busca:** Filtro por nome, estado ou UF
- 🏛️ **Filtro por Estado:** ComboBox para filtrar por estado específico
- ✋ **Validação:** Campos obrigatórios e relacionamentos

**Campos do Formulário:**
- **ID:** Somente leitura (auto-incremento)
- **Nome:** Texto até 100 caracteres (obrigatório)
- **Estado:** ComboBox com lista de estados (obrigatório)

**Campos do DataGrid:**
- ID, Nome da Cidade, Nome do Estado, UF

## 🔧 Funcionalidades Técnicas

### Controle de Estado da Interface

```csharp
private void ConfigurarBotoes()
{
    btnNovo.IsEnabled = true;
    btnSalvar.IsEnabled = false;
    btnEditar.IsEnabled = false;
    btnExcluir.IsEnabled = false;
    btnCancelar.IsEnabled = false;
    
    // Desabilitar campos para edição
    txtNome.IsEnabled = false;
    txtUF.IsEnabled = false; // ou cmbEstado.IsEnabled = false;
}
```

### Tratamento de Erros

- **Try-Catch:** Captura exceções em operações assíncronas
- **MessageBox:** Exibe mensagens de erro amigáveis ao usuário
- **Status Bar:** Feedback visual do status das operações

### Validações

- **Campos Obrigatórios:** Nome, UF (Estados) / Nome, Estado (Cidades)
- **Tamanho de Campos:** Nome (50/100 chars), UF (2 chars)
- **Formatação:** UF automaticamente em maiúsculas
- **Relacionamentos:** Validação de integridade referencial

### Filtros e Buscas

**Estados:**
```csharp
// Busca por nome ou UF (case-insensitive)
var estadosFiltrados = _estados.Where(e => 
    e.Nome.ToLower().Contains(filtro) || 
    e.UF.ToLower().Contains(filtro));
```

**Cidades:**
```csharp
// Busca por nome, estado ou UF + filtro por estado
var cidadesFiltradas = _cidades.Where(c => 
    c.Nome.ToLower().Contains(filtroTexto) ||
    c.Estado?.Nome.ToLower().Contains(filtroTexto) ||
    c.Estado?.UF.ToLower().Contains(filtroTexto))
    .Where(c => estadoSelecionado == 0 || c.EstadoId == estadoSelecionado);
```

## 🔄 Fluxo de Trabalho

### Adicionar Novo Registro

1. **Usuário:** Clica em "Novo"
2. **Sistema:** Habilita campos e botão "Salvar"
3. **Usuário:** Preenche formulário
4. **Usuário:** Clica em "Salvar"
5. **Sistema:** Valida dados
6. **Sistema:** Chama service → repository → database
7. **Sistema:** Atualiza lista e exibe sucesso

### Editar Registro

1. **Usuário:** Seleciona item no DataGrid
2. **Sistema:** Preenche formulário e habilita "Editar"
3. **Usuário:** Clica em "Editar"
4. **Sistema:** Habilita campos para edição
5. **Usuário:** Modifica dados e clica "Salvar"
6. **Sistema:** Atualiza registro e lista

### Excluir Registro

1. **Usuário:** Seleciona item e clica "Excluir"
2. **Sistema:** Exibe confirmação
3. **Usuário:** Confirma exclusão
4. **Sistema:** Remove do banco e atualiza lista

## 🎯 Características Especiais

### ObservableCollection

Utilização de `ObservableCollection<T>` para binding automático com o DataGrid:

```csharp
private ObservableCollection<Estado> _estados;
private ObservableCollection<Estado> _estadosFiltrados;
```

### Operações Assíncronas

Todas as operações de banco são assíncronas:

```csharp
private async Task CarregarEstadosAsync()
{
    var estados = await _estadoService.ObterTodosAsync();
    // ...
}
```

### Confirmação de Fechamento

Proteção contra perda de dados não salvos:

```csharp
protected override void OnClosing(CancelEventArgs e)
{
    if (_modoEdicao)
    {
        var resultado = MessageBox.Show(
            "Existem alterações não salvas. Deseja realmente fechar?",
            "Confirmação", MessageBoxButton.YesNo);
        
        if (resultado == MessageBoxResult.No)
            e.Cancel = true;
    }
}
```

## 🛠️ Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **WPF** - Interface gráfica
- **Entity Framework Core** - ORM
- **SQLite** - Banco de dados
- **Microsoft.Extensions.DependencyInjection** - Injeção de dependências
- **Microsoft.Extensions.Configuration** - Configurações
- **C# 12** - Linguagem de programação
- **XAML** - Definição de interface

## 🧪 Testes

Para testar o aplicativo:

1. **Compile:** `dotnet build .\Projeto.WPF\Projeto.WPF.csproj`
2. **Execute:** `dotnet run --project .\Projeto.WPF\Projeto.WPF.csproj`
3. **Teste Estados:**
   - Adicione novos estados (ex: "Bahia", "BA")
   - Edite estados existentes
   - Teste a busca por nome/UF
   - Tente excluir um estado
4. **Teste Cidades:**
   - Adicione cidades para diferentes estados
   - Use o filtro por estado
   - Teste busca por nome/estado/UF
   - Edite e exclua cidades

## 📝 Observações

- O projeto compartilha o banco de dados com `Projeto.Web`
- Todas as validações seguem as mesmas regras do projeto Web
- A interface é responsiva e adapta-se ao redimensionamento
- O sistema mantém estado consistente entre as operações
- Implementação completa do padrão MVVM implícito via binding

## 🤝 Integração com a Solução

Este projeto WPF integra-se perfeitamente com os outros projetos da solução:

- **Projeto.RegrasDeNegocio:** Reutiliza todos os services e models
- **Projeto.Infraestrutura:** Utiliza os mesmos repositories e DbContext  
- **Projeto.Web:** Compartilha o banco de dados SQLite

Isso garante consistência de dados e reutilização máxima de código entre as diferentes interfaces (Web e Desktop).