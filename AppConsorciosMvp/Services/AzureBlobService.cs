using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AppConsorciosMvp.Services
{
    /// <summary>
    /// Serviço para gerenciar arquivos no Azure Blob Storage
    /// </summary>
    public class AzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");
            _containerName = configuration["AzureStorage:ContainerName"] ?? "documentos";
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        /// <summary>
        /// Faz upload de um arquivo para o Azure Blob Storage
        /// </summary>
        /// <param name="stream">Stream do arquivo</param>
        /// <param name="fileName">Nome do arquivo</param>
        /// <param name="contentType">Tipo de conteúdo</param>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="tipoDocumento">Tipo do documento</param>
        /// <returns>Informações do blob criado</returns>
        public async Task<(string BlobName, string BlobUrl)> UploadAsync(
            Stream stream, 
            string fileName, 
            string contentType, 
            int usuarioId, 
            string tipoDocumento)
        {
            // Gerar nome único do blob
            var extension = Path.GetExtension(fileName);
            var blobName = $"documentos/{usuarioId}/{tipoDocumento}/{Guid.NewGuid()}{extension}";

            // Obter container
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            // Fazer upload
            var blobClient = containerClient.GetBlobClient(blobName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            var metadata = new Dictionary<string, string>
            {
                ["usuarioId"] = usuarioId.ToString(),
                ["tipoDocumento"] = tipoDocumento,
                ["nomeOriginal"] = fileName,
                ["uploadDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders,
                Metadata = metadata
            });

            return (blobName, blobClient.Uri.ToString());
        }

        /// <summary>
        /// Faz download de um arquivo do Azure Blob Storage
        /// </summary>
        /// <param name="blobName">Nome do blob</param>
        /// <returns>Stream do arquivo</returns>
        public async Task<(Stream Stream, string ContentType)> DownloadAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"Arquivo não encontrado: {blobName}");
            }

            var downloadResponse = await blobClient.DownloadStreamingAsync();
            return (downloadResponse.Value.Content, downloadResponse.Value.Details.ContentType);
        }

        /// <summary>
        /// Exclui um arquivo do Azure Blob Storage
        /// </summary>
        /// <param name="blobName">Nome do blob</param>
        /// <returns>True se excluído com sucesso</returns>
        public async Task<bool> DeleteAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }

        /// <summary>
        /// Verifica se um arquivo existe
        /// </summary>
        /// <param name="blobName">Nome do blob</param>
        /// <returns>True se existe</returns>
        public async Task<bool> ExistsAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.ExistsAsync();
            return response.Value;
        }

        /// <summary>
        /// Obtém informações de um blob
        /// </summary>
        /// <param name="blobName">Nome do blob</param>
        /// <returns>Propriedades do blob</returns>
        public async Task<BlobProperties?> GetBlobPropertiesAsync(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var response = await blobClient.GetPropertiesAsync();
            return response.Value;
        }
    }
}
