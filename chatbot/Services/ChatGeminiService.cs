using chatbot.Data;
using chatbot.Interfaces;
using chatbot.Models;
using chatbot.Tools;
using GenerativeAI;
using GenerativeAI.Microsoft;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace chatbot.Services;

public class ChatGeminiService : IChatService
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly IChatRepository _repository;
    private readonly IEmbeddingService _embeddingService;
    private readonly VectorSearchService _vectorSearch;
    private readonly AppDbContext _context;


    public ChatGeminiService(
        IConfiguration configuration,
        IChatRepository repository,
        IEmbeddingService embeddingService,
        VectorSearchService vectorSearch,
        AppDbContext context)
    {
        _model = GoogleAIModels.Gemini25Flash;
        _apiKey = configuration["Gemini:ApiKey"] ?? "";
        _repository = repository;
        _embeddingService = embeddingService;
        _vectorSearch = vectorSearch;
        _context = context;
    }

    public async Task<string> SendMessageAsync(string message, Guid conversationId)
    {
        IChatClient cliente = new GenerativeAIChatClient(_apiKey, _model);
        var agent = new ChatClientAgent(cliente);

        var previousMessages = await _repository.GetConversationAsync(conversationId);
        var chatHistory = new List<ChatMessage>
    {
        new ChatMessage(ChatRole.System, @"
            Você é um assistente especializado em vendas.
    Quando precisar consultar o banco, responda APENAS com:
    QUERY=""query_PostgreSQL""
    A estrutura do banco de dados é a seguinte:
            Nome da tabela: ""Sales""
        Coluna     |  Tipo   
    ---------------+---------
     Id            | integer 
     Data          | date    
     Cliente       | text    
     Produto       | text    
     Categoria     | text    
     Quantidade    | integer 
     PrecoUnitario | numeric 
     Total         | numeric 
     Regiao        | text    
     Observacoes   | text    
    Índices:
        ""PK_Sales"" PRIMARY KEY, btree (""Id"")
    A query precisa ser acompanhada de aspas duplas envolvendo o nome da tabela e das colunas desejadas, e strings precisam estar com aspas simples
    Sem explicações extras.
    Valores monetários responda em Reais
    Se não precisar consultar o banco, apenas responda normalmente.
    Não peça query mais de uma vez")
    };

        chatHistory.AddRange(previousMessages.Select(m =>
            new ChatMessage(m.Role == "user" ? ChatRole.User : ChatRole.Assistant, m.Message)));

        chatHistory.Add(new ChatMessage(ChatRole.User, message));

        var run = await agent.RunAsync(chatHistory);
        Boolean isQuery = run.ToString().StartsWith("QUERY");


        while (isQuery)
        {
            var query = run.ToString().Replace("QUERY=\"", "").Replace("\\\"", "\"").Replace("\\'", "'").Replace("\\\\", "\\").Trim();
            query = query.Substring(0, query.Length - 1);

            // executa a query no banco de dados 
            var sqlTool = new RunSQLTool(_context);
            var result = await sqlTool.ExecuteAsync(query);

            chatHistory.Add(new ChatMessage(ChatRole.User, $"Dados da tabela: {result.ToString()}. pergunta do usuário: {message}"));

            // executa nova consulta ao agente
            run = await agent.RunAsync(chatHistory);
            isQuery = run.ToString().StartsWith("QUERY");

            Console.WriteLine("==================================================================================================================");
            Console.WriteLine($"Resposta: {run.ToString()}");
            Console.WriteLine("==================================================================================================================");
            Console.WriteLine($"Resposta: {chatHistory.ToString()}");
        }

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
            Message = run.ToString()
        });

        await _repository.SaveChangesAsync();

        return run.ToString();
    }






    public async Task<List<Conversation>> GetHistoryAsync()
    {
        return await _repository.GetAllConversationsAsync();
    }

    public async Task<List<ChatMessageEntity>> GetConversationAsync(Guid id)
    {
        return await _repository.GetMessagesByConversationIdAsync(id);
    }

    public async Task RemoveConversationAsync(Guid id)
    {
        await _repository.RemoveAllByConversationIdAsync(id);
    }
}
