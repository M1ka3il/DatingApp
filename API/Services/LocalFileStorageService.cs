using API.Interfaces;

namespace API.Services;

// Stores uploaded photos under wwwroot/uploads and serves them as static files.
// Swap this registration for a Cloudinary implementation later without touching callers.
public class LocalFileStorageService(IWebHostEnvironment env) : IFileStorageService
{
  private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
  private const long MaxBytes = 10 * 1024 * 1024; // 10 MB
  private const string UploadsFolder = "uploads";

  public async Task<FileStorageResult> SaveAsync(IFormFile file, string requestOrigin)
  {
    if (file.Length == 0) throw new Exception("Empty file");
    if (file.Length > MaxBytes) throw new Exception("File exceeds the 10MB limit");

    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!AllowedExtensions.Contains(extension))
      throw new Exception("Unsupported file type");

    var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
    var uploadsPath = Path.Combine(webRoot, UploadsFolder);
    Directory.CreateDirectory(uploadsPath);

    var fileName = $"{Guid.NewGuid():N}{extension}";
    var fullPath = Path.Combine(uploadsPath, fileName);

    await using (var stream = File.Create(fullPath))
    {
      await file.CopyToAsync(stream);
    }

    var url = $"{requestOrigin}/{UploadsFolder}/{fileName}";
    return new FileStorageResult(url, null);
  }

  public Task DeleteAsync(string url, string? publicId)
  {
    // Local files: derive the path from the URL's file name.
    var fileName = Path.GetFileName(new Uri(url).LocalPath);
    var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
    var fullPath = Path.Combine(webRoot, UploadsFolder, fileName);

    if (File.Exists(fullPath)) File.Delete(fullPath);
    return Task.CompletedTask;
  }
}
