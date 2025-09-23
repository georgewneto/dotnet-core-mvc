using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Models;

namespace Projeto.RegrasDeNegocio.Services
{
    public class EstadoService : IEstadoService
    {
        private readonly IEstadoRepository _estadoRepository;

        public EstadoService(IEstadoRepository estadoRepository)
        {
            _estadoRepository = estadoRepository;
        }

        public async Task<IEnumerable<Estado>> ObterTodosAsync()
        {
            return await _estadoRepository.ObterTodosAsync();
        }

        public async Task<Estado?> ObterPorIdAsync(int id)
        {
            return await _estadoRepository.ObterPorIdAsync(id);
        }

        public async Task<Estado> AdicionarAsync(Estado estado)
        {
            return await _estadoRepository.AdicionarAsync(estado);
        }

        public async Task AtualizarAsync(Estado estado)
        {
            await _estadoRepository.AtualizarAsync(estado);
        }

        public async Task RemoverAsync(int id)
        {
            await _estadoRepository.RemoverAsync(id);
        }
    }
}