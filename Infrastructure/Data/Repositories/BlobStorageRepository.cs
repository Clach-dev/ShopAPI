using System.Security.Cryptography;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Domain.Interfaces.IRepositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Data.Repositories;

public abstract class BlobStorageRepository : IStorageRepository
{
    private readonly BlobServiceClient _blobServiceClient;
    
    private readonly string _containerName;

    protected BlobStorageRepository(BlobServiceClient blobServiceClient, string containerName)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = containerName;
    }

    public async Task<Uri> UploadFileAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var image = await Image.LoadAsync(fileStream, cancellationToken);
        using var outputStream = new MemoryStream();

        image.Mutate(x => x.AutoOrient());

        IImageEncoder encoder = image.Metadata.DecodedImageFormat.DefaultMimeType switch
        {
            "image/png" => new PngEncoder(),
            _ => new JpegEncoder { Quality = 85 }
        };

        await image.SaveAsync(outputStream, encoder, cancellationToken);
        outputStream.Position = 0;

        using var sha256 = SHA256.Create();
        var hash = BitConverter.ToString(sha256.ComputeHash(outputStream.ToArray())).Replace("-", "").ToLower();
        var fileExtension = encoder switch
        {
            JpegEncoder => "jpg",
            PngEncoder => "png",
            _ => "jpg"
        };
    
        var blobClient = containerClient.GetBlobClient($"{hash}.{fileExtension}");
    
        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            await blobClient.UploadAsync(outputStream, overwrite: false, cancellationToken: cancellationToken);
        }
    
        return blobClient.Uri;
    }
    
    public async Task DeleteFileAsync(Uri imageUri, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var fileName = imageUri.Segments[^1];
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
    
    public Uri GetReadOnlyImageUri(Uri imageUri)
    {
        var blobClient = _blobServiceClient.GetBlobContainerClient(_containerName)
            .GetBlobClient(imageUri.Segments[^1]);

        if (!blobClient.CanGenerateSasUri) return imageUri;
        
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(60)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        return blobClient.GenerateSasUri(sasBuilder);
    }
}