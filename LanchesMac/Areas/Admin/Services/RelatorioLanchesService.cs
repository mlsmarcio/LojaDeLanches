using LanchesMac.Context;
using LanchesMac.Models;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Areas.Admin.Services
{
    public class RelatorioLanchesService
    {
        private readonly AppDbContext _context;

        public RelatorioLanchesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Lanche>> GetLanchesReport()
        {
            var lanche = await _context.Lanches.ToListAsync();
            if (lanche is null)
            {
                return Enumerable.Empty<Lanche>();
                //return default(IEnumerable<Lanche>);
            }
            return lanche;
        }
        public async Task<IEnumerable<Categoria>> GetCategoriasReport()
        {
            var categorias = await _context.Categorias.ToListAsync();
            if (categorias is null)
            {
                return Enumerable.Empty<Categoria>();
                //return default(IEnumerable<Categoria>);
            }
            return categorias;
        }
    }
}
