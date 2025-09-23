using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Models;

namespace Projeto.RegrasDeNegocio.Services
{
    public class CidadeService : ICidadeService
    {
        private readonly ICidadeRepository _cidadeRepository;

        public CidadeService(ICidadeRepository cidadeRepository)
        {
            _cidadeRepository = cidadeRepository;
        }

        public async Task<IEnumerable<Cidade>> ObterTodosAsync()
        {
            return await _cidadeRepository.ObterTodosAsync();
        }

        public async Task<Cidade?> ObterPorIdAsync(int id)
        {
            return await _cidadeRepository.ObterPorIdAsync(id);
        }

        public async Task<IEnumerable<Cidade>> ObterPorEstadoIdAsync(int estadoId)
        {
            return await _cidadeRepository.ObterPorEstadoIdAsync(estadoId);
        }

        public async Task<Cidade> AdicionarAsync(Cidade cidade)
        {
            return await _cidadeRepository.AdicionarAsync(cidade);
        }

        public async Task AtualizarAsync(Cidade cidade)
        {
            await _cidadeRepository.AtualizarAsync(cidade);
        }

        public async Task RemoverAsync(int id)
        {
            await _cidadeRepository.RemoverAsync(id);
        }
    }
}