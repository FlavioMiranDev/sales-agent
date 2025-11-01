using chatbot.Models;

namespace chatbot.Interfaces;

public interface IChatRepository
{
    Task<List<ChatMessageEntity>> GetConversationAsync(Guid conversationId);
    Task AddMessageAsync(ChatMessageEntity message);
    Task<List<Conversation>> GetAllConversationsAsync();
    Task<List<ChatMessageEntity>> GetMessagesByConversationIdAsync(Guid conversatioId);
    Task SaveChangesAsync();
}
