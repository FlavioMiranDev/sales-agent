using chatbot.Models;
using Pgvector;

namespace chatbot.Interfaces;

public interface IEmbeddingRepository
{
    Task<EmbeddingData?> GetByIdAsync(Guid id);
    Task<List<EmbeddingData>> GetAllAsync();
    Task<EmbeddingData> AddAsync(EmbeddingData embedding);
    void Update(EmbeddingData embedding);
    Task DeleteAsync(Guid id);
    Task<List<EmbeddingData>> FindSimilarAsync(Vector queryVector, int limit);
    Task AddRange(IEnumerable<EmbeddingData> embeddings);
    Task<int> CountAsync();
    Task DeleteAllAsync();
    Task CommitAsync();
}
