using System.Text;
using chatbot.Data;
using chatbot.Interfaces;
using chatbot.Services;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Tools;

public class SearchSalesTool
{
    // private readonly IEmbeddingService _embeddingService;
    // private readonly VectorSearchService _vectorSearchService;
    private readonly AppDbContext _context;

    public SearchSalesTool(AppDbContext context)
    {
        // _embeddingService = embeddingService;
        // _vectorSearchService = vectorSearchService;
        _context = context;
    }

    public async Task<string> ExecuteAsync(string query)
    {
        // Busca textual simples — contém no cliente, produto, região ou observações
        var sales = await _context.Sales
            .Where(s =>
                EF.Functions.ILike(s.Cliente, $"%{query}%") ||
                EF.Functions.ILike(s.Produto, $"%{query}%") ||
                EF.Functions.ILike(s.Regiao, $"%{query}%") ||
                EF.Functions.ILike(s.Observacoes, $"%{query}%") ||
                EF.Functions.ILike(s.Categoria, $"%{query}%"))
            .Take(10)
            .ToListAsync();

        if (sales.Count == 0)
            return $"Nenhuma venda encontrada para: \"{query}\"";

        // Exemplo: retorna texto simples
        var results = sales.Select(s =>
            $"{s.Data}: {s.Cliente} comprou {s.Quantidade}x {s.Produto} (R${s.Total}) na região {s.Regiao}").ToList();

        return string.Join("\n", results);
    }
}
