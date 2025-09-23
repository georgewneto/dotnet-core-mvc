using Projeto.RegrasDeNegocio.Models;

namespace Projeto.RegrasDeNegocio.Interfaces
{
    public interface ICidadeService
    {
        Task<IEnumerable<Cidade>> ObterTodosAsync();
        Task<Cidade?> ObterPorIdAsync(int id);
        Task<IEnumerable<Cidade>> ObterPorEstadoIdAsync(int estadoId);
        Task<Cidade> AdicionarAsync(Cidade cidade);
        Task AtualizarAsync(Cidade cidade);
        Task RemoverAsync(int id);
    }
}