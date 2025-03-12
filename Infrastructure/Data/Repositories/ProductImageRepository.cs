using Azure.Storage.Blobs;
using Domain.Interfaces.IRepositories;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data.Repositories;

public class ProductImageRepository(BlobServiceClient blobServiceClient, IConfiguration configuration)
    : BlobStorageRepository(blobServiceClient, configuration.GetValue<string>("ContainerNameStrings:ProductImageContainer")!),
        IProductImageRepository
{
}