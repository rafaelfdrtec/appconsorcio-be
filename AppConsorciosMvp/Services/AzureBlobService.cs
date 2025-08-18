using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AppConsorciosMvp.Services
{
    /// <summary>
    /// Serviço para gerenciar arquivos no Azure Blob Storage com suporte a múltiplos containers
    /// e nomes definidos via parâmetros de sistema no banco de dados.
    /// </summary>
    public class AzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ParametrosService _parametros;

        // Chaves de parâmetro para containers
        public const string ParamContainerDocumentosUsuarios = "Azure.Container.DocumentosUsuarios";
        public const string ParamContainerAnexosPropostas = "Azure.Container.AnexosPropostas";
        public const string ParamContainerAnexosCartas = "Azure.Container.AnexosCartas";
        public const string ParamContainerDefault = "Azure.Container.Default";

        public AzureBlobService(IConfiguration configuration, ParametrosService parametros)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException("String de conexão do Azure Storage não configurada ('AzureStorage').");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _parametros = parametros;
        }

        // ========= Uploads especializados =========

        /// <summary>
        /// Upload de documento de usuário (PII) para o container específico configurado
        /// </summary>
        public async Task<(string BlobName, string BlobUrl)> UploadDocumentoUsuarioAsync(
            Stream stream,
            string fileName,
            string contentType,
            int usuarioId,
            string tipoDocumento)
        {
            var containerName = await _parametros.GetValorOrDefaultAsync(ParamContainerDocumentosUsuarios, "documentos-usuarios");
            var extension = Path.GetExtension(fileName);
            var blobName = $"usuarios/{usuarioId}/{tipoDocumento}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}{extension}";

            var metadata = new Dictionary<string, string>
            {
                ["usuarioId"] = usuarioId.ToString(),
                ["tipoDocumento"] = tipoDocumento,
                ["nomeOriginal"] = fileName,
                ["uploadDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return await UploadToContainerAsync(containerName, stream, blobName, contentType, metadata);
        }

        /// <summary>
        /// Upload de anexo de proposta
        /// </summary>
        public async Task<(string BlobName, string BlobUrl)> UploadAnexoPropostaAsync(
            Stream stream,
            string fileName,
            string contentType,
            int propostaId)
        {
            var containerName = await _parametros.GetValorOrDefaultAsync(ParamContainerAnexosPropostas, "anexos-propostas");
            var extension = Path.GetExtension(fileName);
            var blobName = $"propostas/{propostaId}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}{extension}";

            var metadata = new Dictionary<string, string>
            {
                ["propostaId"] = propostaId.ToString(),
                ["nomeOriginal"] = fileName,
                ["uploadDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return await UploadToContainerAsync(containerName, stream, blobName, contentType, metadata);
        }

        /// <summary>
        /// Upload de anexo de carta
        /// </summary>
        public async Task<(string BlobName, string BlobUrl)> UploadAnexoCartaAsync(
            Stream stream,
            string fileName,
            string contentType,
            int cartaId)
        {
            var containerName = await _parametros.GetValorOrDefaultAsync(ParamContainerAnexosCartas, "anexos-cartas");
            var extension = Path.GetExtension(fileName);
            var blobName = $"cartas/{cartaId}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}{extension}";

            var metadata = new Dictionary<string, string>
            {
                ["cartaId"] = cartaId.ToString(),
                ["nomeOriginal"] = fileName,
                ["uploadDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return await UploadToContainerAsync(containerName, stream, blobName, contentType, metadata);
        }

        // ========= Métodos genéricos =========

        private async Task<(string BlobName, string BlobUrl)> UploadToContainerAsync(
            string containerName,
            Stream stream,
            string blobName,
            string contentType,
            IDictionary<string, string>? metadata = null)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobClient = containerClient.GetBlobClient(blobName);

            var headers = new BlobHttpHeaders { ContentType = contentType };

            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = headers,
                Metadata = metadata
            });

            return (blobName, blobClient.Uri.ToString());
        }

        /// <summary>
        /// Download usando chave de parâmetro do container
        /// </summary>
        public async Task<(Stream Stream, string ContentType)> DownloadAsync(string containerParamKey, string blobName)
        {
            var containerName = await _parametros.GetValorOrDefaultAsync(containerParamKey, await _parametros.GetValorOrDefaultAsync(ParamContainerDefault, "documentos-usuarios"));
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException($"Arquivo não encontrado: {blobName}");

            var response = await blobClient.DownloadStreamingAsync();
            return (response.Value.Content, response.Value.Details.ContentType);
        }

        /// <summary>
        /// Delete usando chave de parâmetro do container
        /// </summary>
        public async Task<bool> DeleteAsync(string containerParamKey, string blobName)
        {
            var containerName = await _parametros.GetValorOrDefaultAsync(containerParamKey, await _parametros.GetValorOrDefaultAsync(ParamContainerDefault, "documentos-usuarios"));
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            var result = await blobClient.DeleteIfExistsAsync();
            return result.Value;
        }

        /// <summary>
        /// Verifica existência usando chave de parâmetro do container
        /// </summary>
        public async Task<bool> ExistsAsync(string containerParamKey, string blobName)
        {
            var containerName = await _parametros.GetValorOrDefaultAsync(containerParamKey, await _parametros.GetValorOrDefaultAsync(ParamContainerDefault, "documentos-usuarios"));
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            var resp = await blobClient.ExistsAsync();
            return resp.Value;
        }

        /// <summary>
        /// Propriedades de blob usando chave de parâmetro do container
        /// </summary>
        public async Task<BlobProperties?> GetBlobPropertiesAsync(string containerParamKey, string blobName)
        {
            var containerName = await _parametros.GetValorOrDefaultAsync(containerParamKey, await _parametros.GetValorOrDefaultAsync(ParamContainerDefault, "documentos-usuarios"));
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            if (!await blobClient.ExistsAsync()) return null;
            var response = await blobClient.GetPropertiesAsync();
            return response.Value;
        }

        // ========= Compatibilidade com método antigo (documentos de usuário) =========
        public Task<(string BlobName, string BlobUrl)> UploadAsync(Stream stream, string fileName, string contentType, int usuarioId, string tipoDocumento)
            => UploadDocumentoUsuarioAsync(stream, fileName, contentType, usuarioId, tipoDocumento);
    }
}
