namespace chatbot.Interfaces;

public interface IEmbeddingService
{
    Task<float[][]> GetManyEmbeddingsAsync(List<string> texts);
    Task<float[]> GetEmbeddings(string text);
}
