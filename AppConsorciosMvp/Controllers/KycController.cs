using System.Security.Claims;
using AppConsorciosMvp.Data;
using AppConsorciosMvp.Models.Mvp;
using AppConsorciosMvp.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    [Route("kyc")]
    [Authorize]
    public class KycController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IFileStorage _storage;

        public KycController(AppDbContext db, IFileStorage storage)
        {
            _db = db;
            _storage = storage;
        }

        [HttpPost("start")]
        public async Task<ActionResult<object>> Start([FromBody] StartKycRequest body)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var kyc = new KycCase
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LevelRequested = body.levelRequested,
                Status = "pending",
                EvidenceRefs = "[]",
                Severity = "informative",
                BlocksJson = "[]"
            };
            _db.KycCases.Add(kyc);
            await _db.SaveChangesAsync();
            return Created(string.Empty, new { kycCaseId = kyc.Id });
        }

        [HttpPost("upload-url")]
        public async Task<ActionResult<object>> UploadUrl([FromBody] KycUploadUrlRequest body)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var blobPath = $"docs/kyc/{userId}/{DateTime.UtcNow:yyyyMMddHHmmss}-{body.docType}.bin";
            var sas = await _storage.GetUploadUrlAsync(blobPath, "application/octet-stream", TimeSpan.FromMinutes(10));
            return Ok(new { uploadUrl = sas.Url, expiresAt = sas.ExpiresAt });
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var caseOpen = await _db.KycCases.Where(k => k.UserId == userId && k.Status == "pending")
                .OrderByDescending(k => k.DueAt)
                .FirstOrDefaultAsync();
            if (caseOpen == null) return NotFound();

            // Mantém pendente para revisão manual
            return NoContent();
        }

        public record StartKycRequest(int levelRequested, string role);
        public record KycUploadUrlRequest(string docType);
    }
}
