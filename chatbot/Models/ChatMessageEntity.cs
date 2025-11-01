namespace chatbot.Models;

public class ChatMessageEntity
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
