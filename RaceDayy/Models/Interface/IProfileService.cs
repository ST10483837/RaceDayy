namespace RaceDayy.Models.Interface
{
    public interface IProfileImageService
    {
        Task<string?> SaveAsync(IFormFile file);

        void Delete(string? relativeFilePath);
    }
    public class ProfileImageService : IProfileImageService
    {
        private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxBytes = 4 * 1024 * 1024; // 4 MB
        private readonly IWebHostEnvironment _env;

        public ProfileImageService(IWebHostEnvironment env) => _env = env;

        public async Task<string?> SaveAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!Allowed.Contains(ext) || file.Length == 0 || file.Length > MaxBytes)
                return null;

            var Folder = Path.Combine(_env.WebRootPath, "uploads", "profile");
            Directory.CreateDirectory(Folder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(Folder, fileName);

            await using var Stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(Stream);

            return $"/uploads/profile/{fileName}";
        }
        public void Delete(string? relativeFilePath)
        {
            if (string.IsNullOrEmpty(relativeFilePath)) return;

            var fullPath = Path.Combine(
                _env.WebRootPath,
                relativeFilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}