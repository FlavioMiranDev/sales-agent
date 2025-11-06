using chatbot.Models;

namespace chatbot.Interfaces;

public interface IChatService
{
    Task<string> SendMessageAsync(string message, Guid conversationId);
    Task<List<Conversation>> GetHistoryAsync();
    Task<List<ChatMessageEntity>> GetConversationAsync(Guid id);
    Task RemoveConversationAsync(Guid id);
}
