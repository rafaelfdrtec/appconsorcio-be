namespace AppConsorciosMvp.Storage
{
    public interface IFileStorage
    {
        Task<SasResult> GetUploadUrlAsync(string path, string contentType, TimeSpan ttl);
        Task<SasResult> GetDownloadUrlAsync(string path, TimeSpan ttl);
    }

    public record SasResult(string Url, DateTimeOffset ExpiresAt);
}
