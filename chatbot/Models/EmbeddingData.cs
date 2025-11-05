using Pgvector;

namespace chatbot.Models;

public class EmbeddingData
{
    public int Id { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public Vector Vector { get; set; } = null!;
}