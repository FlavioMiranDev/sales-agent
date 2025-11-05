using chatbot.Interfaces;
using GenerativeAI;
using GenerativeAI.Microsoft;
using Microsoft.Extensions.AI;

namespace chatbot.Services;

public class EmbeddingGeminiService : IEmbeddingService
{
    private readonly GenerativeAIEmbeddingGenerator _embeddingGenerator;

    public EmbeddingGeminiService(IConfiguration configuration)
    {
        _embeddingGenerator = new GenerativeAIEmbeddingGenerator(
                                configuration["Gemini:ApiKey"] ?? "",
                                "models/text-embedding-004");
    }

    public async Task<float[][]> GetManyEmbeddingsAsync(List<string> texts)
    {
        var embeddingsTasks = texts.Select(text => _embeddingGenerator.GenerateVectorAsync(text));
        var embeddingsResults = await Task.WhenAll(embeddingsTasks);

        return embeddingsResults.Select(e => e.ToArray()).ToArray();
    }

    public async Task<float[]> GetEmbeddings(string text)
    {
        var embeddings = await _embeddingGenerator.GenerateVectorAsync(text);

        return embeddings.ToArray();
    }
}
