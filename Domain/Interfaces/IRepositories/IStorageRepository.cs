namespace Domain.Interfaces.IRepositories;

public interface IStorageRepository
{
    Task<Uri> UploadFileAsync(Stream fileStream, CancellationToken cancellationToken);
    
    Task DeleteFileAsync(Uri uri, CancellationToken cancellationToken);
    
    Uri GetReadOnlyImageUri(Uri imageUri);
}   