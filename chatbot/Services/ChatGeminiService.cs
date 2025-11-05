using chatbot.Interfaces;
using chatbot.Models;
using chatbot.Tools;
using GenerativeAI;
using GenerativeAI.Microsoft;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace chatbot.Services;

public class ChatGeminiService : IChatService
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly IChatRepository _repository;
    private readonly IEmbeddingService _embeddingService;
    private readonly VectorSearchService _vectorSearch;

    public ChatGeminiService(
        IConfiguration configuration,
        IChatRepository repository,
        IEmbeddingService embeddingService,
        VectorSearchService vectorSearch)
    {
        _model = GoogleAIModels.Gemini25Flash;
        _apiKey = configuration["Gemini:ApiKey"] ?? "";
        _repository = repository;
        _embeddingService = embeddingService;
        _vectorSearch = vectorSearch;
    }

    public async Task<string> SendMessageAsync(string message, Guid conversationId)
    {
        IChatClient cliente = new GenerativeAIChatClient(_apiKey, _model);

        var previousMessages = await _repository.GetConversationAsync(conversationId);

        var chatHistory = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System,
                        @"Você é assistente que deve responder a todas as perguntas do usuário com no máximo 700 caracteres, sempre respondendo de forma resumida e direta")
        };

        chatHistory.AddRange(previousMessages
                            .Select(m => new ChatMessage(
                                m.Role == "user" ? ChatRole.User : ChatRole.Assistant,
                                m.Message)));

        chatHistory.Add(new ChatMessage(ChatRole.User, message));

        AIAgent agent = new ChatClientAgent(cliente);
        AgentRunResponse response = await agent.RunAsync(chatHistory);

        var responseForUser = response.ToString() ?? "Sorry, I couldn't process your message";

        await _repository.AddMessageAsync(new ChatMessageEntity
        {
            ConversationId = conversationId,
            Role = "user",
            Message = message
        });

        await _repository.AddMessageAsync(new ChatMessageEntity
        {
            ConversationId = conversationId,
            Role = "assistant",
            Message = responseForUser
        });

        await _repository.SaveChangesAsync();

        return responseForUser;
    }

    public async Task<List<Conversation>> GetHistoryAsync()
    {
        return await _repository.GetAllConversationsAsync();
    }

    public async Task<List<ChatMessageEntity>> GetConversationAsync(Guid id)
    {
        return await _repository.GetMessagesByConversationIdAsync(id); ;
    }

    public async Task RemoveConversationAsync(Guid id)
    {
        await _repository.RemoveAllByConversationIdAsync(id);
    }
}