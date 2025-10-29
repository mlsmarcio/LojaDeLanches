using LanchesMac.Context;
using LanchesMac.Models;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Areas.Admin.Services
{
    public class RelatorioVendasService
    {
        private readonly AppDbContext _appDbContext;

        public RelatorioVendasService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Pedido>> FindByDateAsync(DateTime? mindate, DateTime? maxDate)
        {
            // USO DE IQUERYABLE SEMPRE QUANDO FOR EXPANDIR A CONSULTA
            var resultado = from obj in _appDbContext.Pedidos select obj;

            if (mindate.HasValue)
            {
                resultado = resultado.Where(x => x.PedidoEnviado >= mindate.Value);
            }
            if (maxDate.HasValue)
            {
                resultado = resultado.Where(x => x.PedidoEnviado <= maxDate.Value);
            }

            return await resultado
                .Include(l => l.PedidoItens)
                .ThenInclude(l => l.Lanche)
                .OrderByDescending(x => x.PedidoEnviado)
                .ToListAsync();
        }
    }
}
