using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace AppConsorciosMvp.Storage
{
    public class AzureSasFileStorage : IFileStorage
    {
        private readonly BlobServiceClient _serviceClient;
        private readonly string _defaultContainer;

        public AzureSasFileStorage(IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException("AzureStorage connection string não configurada.");
            _serviceClient = new BlobServiceClient(conn);
            _defaultContainer = configuration["Blob:Container"] ?? "docs";
        }

        public async Task<SasResult> GetUploadUrlAsync(string path, string contentType, TimeSpan ttl)
        {
            var (containerName, blobName) = ResolvePath(path);
            var container = _serviceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            var client = container.GetBlobClient(blobName);
            var expires = DateTimeOffset.UtcNow.Add(ttl);

            var builder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = expires,
                ContentType = contentType
            };
            builder.SetPermissions(GetPermissions(write: true));
            var sas = client.GenerateSasUri(builder);

            return new SasResult(sas.ToString(), expires);
        }

        public async Task<SasResult> GetDownloadUrlAsync(string path, TimeSpan ttl)
        {
            var (containerName, blobName) = ResolvePath(path);
            var container = _serviceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            var client = container.GetBlobClient(blobName);
            var expires = DateTimeOffset.UtcNow.Add(ttl);

            var builder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = expires
            };
            builder.SetPermissions(GetPermissions(read: true));
            var sas = client.GenerateSasUri(builder);

            return new SasResult(sas.ToString(), expires);
        }

        // Removed unused GetCredentials method: obsolete/unsupported API call was causing CS1061.

        private BlobSasPermissions GetPermissions(bool read = false, bool write = false)
        {
            var perms = default(BlobSasPermissions);
            if (read) perms |= BlobSasPermissions.Read;
            if (write) perms |= BlobSasPermissions.Write | BlobSasPermissions.Create;
            return perms;
        }

        private (string container, string blob) ResolvePath(string path)
        {
            // Permite path no formato "container/blob" ou apenas "blob" (usa default)
            var parts = path.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2) return (parts[0], parts[1]);
            return (_defaultContainer, parts[0]);
        }
    }
}
