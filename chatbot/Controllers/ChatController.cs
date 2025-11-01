using chatbot.Interfaces;
using chatbot.Models;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IChatRepository _chatRepository;

    public ChatController(IChatService chatService, IChatRepository chatRepository)
    {
        _chatService = chatService;
        _chatRepository = chatRepository;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("The message don't be empty");
        }

        Guid conversationId;

        if (!Guid.TryParse(request.ConversationId ?? "", out conversationId))
        {
            conversationId = Guid.NewGuid();
        }

        var response = await _chatService.SendMessageAsync(request.Message, conversationId);

        return Ok(new ChatMessageResponse
        {
            Response = response,
            ConversationId = conversationId,
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet]
    public async Task<IActionResult> History()
    {
        var conversations = await _chatRepository.GetAllConversationsAsync();

        return Ok(conversations);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Conversation(Guid id)
    {
        var conversation = await _chatRepository.GetMessagesByConversationIdAsync(id);

        return Ok(conversation);
    }
}
