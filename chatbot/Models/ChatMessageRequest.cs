namespace chatbot.Models;

public class ChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
}
