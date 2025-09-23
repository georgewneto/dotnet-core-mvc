using Microsoft.EntityFrameworkCore;
using Projeto.Infraestrutura.Data;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Models;

namespace Projeto.Infraestrutura.Repositories
{
    public class EstadoRepository : IEstadoRepository
    {
        private readonly ApplicationDbContext _context;

        public EstadoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Estado>> ObterTodosAsync()
        {
            return await _context.Estados.AsNoTracking().ToListAsync();
        }

        public async Task<Estado?> ObterPorIdAsync(int id)
        {
            return await _context.Estados
                .Include(e => e.Cidades)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Estado> AdicionarAsync(Estado estado)
        {
            _context.Estados.Add(estado);
            await _context.SaveChangesAsync();
            return estado;
        }

        public async Task AtualizarAsync(Estado estado)
        {
            _context.Estados.Update(estado);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var estado = await _context.Estados.FindAsync(id);
            if (estado != null)
            {
                _context.Estados.Remove(estado);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Estados.AnyAsync(e => e.Id == id);
        }
    }
}