namespace chatbot.Models;

public class ChatMessageResponse
{
    public string Response { get; set; } = string.Empty;
    public Guid ConversationId { get; set; } = Guid.Empty;
    public DateTime Timestamp { get; set; }
}
