using Microsoft.EntityFrameworkCore;
using Projeto.Infraestrutura.Data;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Models;

namespace Projeto.Infraestrutura.Repositories
{
    public class CidadeRepository : ICidadeRepository
    {
        private readonly ApplicationDbContext _context;

        public CidadeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cidade>> ObterTodosAsync()
        {
            return await _context.Cidades
                .Include(c => c.Estado)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cidade?> ObterPorIdAsync(int id)
        {
            return await _context.Cidades
                .Include(c => c.Estado)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cidade>> ObterPorEstadoIdAsync(int estadoId)
        {
            return await _context.Cidades
                .Where(c => c.EstadoId == estadoId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cidade> AdicionarAsync(Cidade cidade)
        {
            _context.Cidades.Add(cidade);
            await _context.SaveChangesAsync();
            return cidade;
        }

        public async Task AtualizarAsync(Cidade cidade)
        {
            _context.Cidades.Update(cidade);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var cidade = await _context.Cidades.FindAsync(id);
            if (cidade != null)
            {
                _context.Cidades.Remove(cidade);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Cidades.AnyAsync(c => c.Id == id);
        }
    }
}