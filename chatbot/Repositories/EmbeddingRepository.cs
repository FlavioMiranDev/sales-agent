using chatbot.Data;
using chatbot.Interfaces;
using chatbot.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace chatbot.Repositories;

public class EmbeddingRepository : IEmbeddingRepository
{
    private readonly AppDbContext _context;

    public EmbeddingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmbeddingData?> GetByIdAsync(Guid id)
    {
        return await _context.EmbeddingData.FindAsync(id);
    }

    public async Task<List<EmbeddingData>> GetAllAsync()
    {
        return await _context.EmbeddingData.ToListAsync();
    }

    public async Task<EmbeddingData> AddAsync(EmbeddingData embedding)
    {
        await _context.EmbeddingData.AddAsync(embedding);

        return embedding;
    }

    public void Update(EmbeddingData embedding)
    {
        _context.EmbeddingData.Update(embedding);
    }

    public async Task DeleteAsync(Guid id)
    {
        var embedding = await GetByIdAsync(id);

        if (embedding is null) return;

        _context.EmbeddingData.Remove(embedding);
    }

    public async Task<List<EmbeddingData>> FindSimilarAsync(Vector queryVector, int limit)
    {
        return await _context.EmbeddingData
            .OrderBy(e => e.Vector.CosineDistance(queryVector))
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddRange(IEnumerable<EmbeddingData> embeddings)
    {
        await _context.EmbeddingData.AddRangeAsync(embeddings);
    }

    public async Task<int> CountAsync()
    {
        return await _context.EmbeddingData.CountAsync();
    }

    public async Task DeleteAllAsync()
    {
        var all = await _context.EmbeddingData.ToListAsync();

        _context.EmbeddingData.RemoveRange(all);
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}
