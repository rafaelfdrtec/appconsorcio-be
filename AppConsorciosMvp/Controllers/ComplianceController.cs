using AppConsorciosMvp.Data;
using AppConsorciosMvp.Models.Mvp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    [Route("compliance")]
    [Authorize(Roles = "admin,compliance")]
    public class ComplianceController(AppDbContext db) : ControllerBase
    {
        [HttpGet("kyc")]
        public async Task<ActionResult<IEnumerable<object>>> List([FromQuery] string? status)
        {
            var q = db.KycCases.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(k => k.Status == status);
            var list = await q.OrderByDescending(k => k.DueAt).Take(200).ToListAsync();
            return Ok(list.Select(k => new
            {
                kycCaseId = k.Id,
                user = k.UserId,
                levelRequested = k.LevelRequested,
                evidence = k.EvidenceRefs,
                score = k.Score
            }));
        }

        [HttpPost("kyc/{id:guid}/approve")]
        public async Task<IActionResult> Approve([FromRoute] Guid id)
        {
            var k = await db.KycCases.FindAsync(id);
            if (k == null) return NotFound();
            k.Status = "approved";
            var user = await db.Usuarios.FindAsync(k.UserId);
            if (user != null) user.KycLevel = Math.Max(user.KycLevel, k.LevelRequested);
            await db.SaveChangesAsync();
            return NoContent();
        }

        public record RejectBody(string ReasonCode, string Message, string Severity, DateTimeOffset? DueAt, string[] Blocks);

        [HttpPost("kyc/{id:guid}/reject")]
        public async Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] RejectBody body)
        {
            var k = await db.KycCases.FindAsync(id);
            if (k == null) return NotFound();
            k.Status = "rejected";
            k.ReasonCode = body.ReasonCode;
            k.ReasonMessage = body.Message;
            k.Severity = body.Severity;
            k.DueAt = body.DueAt;
            k.BlocksJson = System.Text.Json.JsonSerializer.Serialize(body.Blocks);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
