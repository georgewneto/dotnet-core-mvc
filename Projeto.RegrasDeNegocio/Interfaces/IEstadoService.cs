using Projeto.RegrasDeNegocio.Models;

namespace Projeto.RegrasDeNegocio.Interfaces
{
    public interface IEstadoService
    {
        Task<IEnumerable<Estado>> ObterTodosAsync();
        Task<Estado?> ObterPorIdAsync(int id);
        Task<Estado> AdicionarAsync(Estado estado);
        Task AtualizarAsync(Estado estado);
        Task RemoverAsync(int id);
    }
}