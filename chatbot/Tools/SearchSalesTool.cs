using System.Text;
using chatbot.Interfaces;
using chatbot.Services;

namespace chatbot.Tools;

public class SearchSalesTool
{
    private readonly IEmbeddingService _embeddingService;
    private readonly VectorSearchService _vectorSearchService;

    public SearchSalesTool(IEmbeddingService embeddingService, VectorSearchService vectorSearchService)
    {
        _embeddingService = embeddingService;
        _vectorSearchService = vectorSearchService;
    }

    public async Task<string> ExecuteAsync(string userQuery)
    {
        var queryEmbedding = await _embeddingService.GetEmbeddings(userQuery);
        var similarData = await _vectorSearchService.SearchSimilarAsync(queryEmbedding);

        if (similarData is null || similarData.Count == 0)
            return "Não encontrei informações relevantes nas vendas.";

        var sb = new StringBuilder();

        sb.AppendLine("Aqui estão algumas vendas relevantes encontradas:");

        foreach (var d in similarData)
        {
            sb.AppendLine($"- {d.OriginalText}");
        }

        return sb.ToString();
    }
}
