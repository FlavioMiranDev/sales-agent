namespace chatbot.Interfaces;

public interface IChatService
{
    Task<string> SendMessageAsync(string message, Guid conversationId);
}
