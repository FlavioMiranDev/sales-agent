using chatbot.Interfaces;
using chatbot.Models;
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

    private readonly Dictionary<Guid, List<ChatMessage>> _conversations = new Dictionary<Guid, List<ChatMessage>>();

    public ChatGeminiService(IConfiguration configuration, IChatRepository repository)
    {
        _model = GoogleAIModels.Gemini25Flash;
        _apiKey = configuration["Gemini:ApiKey"] ?? "";
        _repository = repository;
    }

    public async Task<string> SendMessageAsync(string message, Guid conversationId)
    {
        IChatClient cliente = new GenerativeAIChatClient(_apiKey, _model);

        var previousMessages = await _repository.GetConversationAsync(conversationId);


        var chatHistory = previousMessages
                            .Select(m => new ChatMessage(
                                m.Role == "user" ? ChatRole.User : ChatRole.Assistant,
                                m.Message))
                            .ToList();

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
}