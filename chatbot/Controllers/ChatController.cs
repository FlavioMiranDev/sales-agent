using chatbot.Interfaces;
using chatbot.Models;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
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
        return Ok(await _chatService.GetHistoryAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Conversation(Guid id)
    {
        return Ok(await _chatService.GetConversationAsync(id));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveConversations(Guid id)
    {
        await _chatService.RemoveConversationAsync(id);

        return Ok();
    }
}
