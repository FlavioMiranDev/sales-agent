using chatbot.Data;
using chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Services;

public class VectorSearchService
{
    private readonly AppDbContext _context;

    public VectorSearchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmbeddingData>> SearchSimilarAsync(float[] queryEmbedding, int limit = 3)
    {
        var results = await _context.EmbeddingData
            .FromSqlRaw("""
                SELECT * FROM "EmbeddingData"
                ORDER BY "Vector" <-> CAST({0} AS vector)
                LIMIT {1}
            """, queryEmbedding, limit)
            .ToListAsync();

        return results;
    }
}
