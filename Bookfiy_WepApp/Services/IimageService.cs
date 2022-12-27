namespace Bookfiy_WepApp.Services
{
    public interface IimageService
    {
        Task<(bool isUploded, string? errorMessage)> UploadAsync(IFormFile image, string imageName, string folderPath, bool hasThumbnail);

        void Delete(string imagePath, string? imageThmbnailPath = null);
    }
}
