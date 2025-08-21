using AppConsorciosMvp.Data;
using AppConsorciosMvp.Models.Mvp;
using AppConsorciosMvp.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    [Route("chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<ChatHub> _chatHub;

        public ChatController(AppDbContext db, IHubContext<ChatHub> chatHub)
        {
            _db = db;
            _chatHub = chatHub;
        }

        [HttpGet("threads")]
        public async Task<ActionResult<IEnumerable<object>>> GetThreads([FromQuery] Guid transacaoId)
        {
            var threads = await _db.ChatThreads.Where(t => t.TransacaoId == transacaoId).ToListAsync();
            return Ok(threads.Select(t => new { t.Id, t.Kind, t.TransacaoId }));
        }

        [HttpGet("messages")]
        public async Task<ActionResult<IEnumerable<object>>> GetMessages([FromQuery] Guid threadId, [FromQuery] int page = 1)
        {
            const int pageSize = 20;
            var msgs = await _db.ChatMessages
                .Where(m => m.ThreadId == threadId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(msgs.Select(m => new
            {
                m.Id,
                m.ThreadId,
                m.AuthorRole,
                m.Type,
                m.Text,
                m.AttachmentsJson,
                m.CreatedAt
            }));
        }

        public record PostMessageBody(Guid threadId, string type, string text, Attachment[]? attachments);
        public record Attachment(string url, string name);

        [HttpPost("messages")]
        public async Task<IActionResult> PostMessage([FromBody] PostMessageBody body)
        {
            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ThreadId = body.threadId,
                AuthorRole = "agent",
                Type = body.type,
                Text = body.text,
                AttachmentsJson = System.Text.Json.JsonSerializer.Serialize(body.attachments ?? Array.Empty<Attachment>()),
                CreatedAt = DateTimeOffset.UtcNow
            };
            _db.ChatMessages.Add(message);
            await _db.SaveChangesAsync();

            await _chatHub.Clients.Group($"thread:{body.threadId}")
                .SendAsync("message", new
                {
                    message.Id,
                    message.ThreadId,
                    message.AuthorRole,
                    message.Type,
                    message.Text,
                    message.AttachmentsJson,
                    message.CreatedAt
                });

            return NoContent();
        }
    }
}
