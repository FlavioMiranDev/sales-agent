using chatbot.Data;
using chatbot.Interfaces;
using chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _context;

    public ChatRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<ChatMessageEntity>> GetConversationAsync(Guid conversationId)
    {
        return await _context.ChatMessages.Where(m => m.ConversationId == conversationId).OrderBy(m => m.CreatedAt).ToListAsync();
    }

    public async Task AddMessageAsync(ChatMessageEntity message)
    {
        await _context.ChatMessages.AddAsync(message);
    }

    public async Task<List<Conversation>> GetAllConversationsAsync()
    {
        return await _context.ChatMessages
            .GroupBy(m => m.ConversationId)
            .Select(g => new Conversation
            {
                ConvesationId = g.Key,
                Title = (
                    g.OrderBy(m => m.CreatedAt)
                     .Select(m => m.Message)
                     .FirstOrDefault() ?? "[sem mensagens]"
                ).Substring(0, Math.Min(
                    (g.OrderBy(m => m.CreatedAt)
                       .Select(m => m.Message)
                       .FirstOrDefault() ?? "[sem mensagens]").Length,
                    100
                ))
            })
            .ToListAsync();
    }

    public async Task<List<ChatMessageEntity>> GetMessagesByConversationIdAsync(Guid conversatioId)
    {
        return await _context.ChatMessages
            .Where(m => m.ConversationId == conversatioId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task RemoveAllByConversationIdAsync(Guid conversationId)
    {
        await _context.ChatMessages
            .Where(m => m.ConversationId == conversationId)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
