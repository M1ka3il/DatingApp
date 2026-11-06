namespace API.Interfaces;

public record FileStorageResult(string Url, string? PublicId);

public interface IFileStorageService
{
  // Saves the uploaded file and returns its publicly reachable URL.
  Task<FileStorageResult> SaveAsync(IFormFile file, string requestOrigin);

  // Removes a previously stored file. publicId is null for local files.
  Task DeleteAsync(string url, string? publicId);
}
