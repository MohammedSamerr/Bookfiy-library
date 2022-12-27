using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Bookfiy_WepApp.Services
{
    public class ImageService : IimageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private int _maxAllowedSize = 2097152;
        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        #region Delete
        public void Delete(string imagePath, string? imageThmbnailPath = null)
        {
            var oldImage = $"{_webHostEnvironment.WebRootPath}{imagePath}";
            
            if (File.Exists(oldImage))
                File.Delete(oldImage);

            if (!string.IsNullOrEmpty(imageThmbnailPath))
            {
                var oldThumbPath = $"{_webHostEnvironment.WebRootPath}{imageThmbnailPath}";
                if (File.Exists(oldThumbPath))
                    File.Delete(oldThumbPath);
            }
            
        }
        #endregion

        #region upload
        public async Task<(bool isUploded, string? errorMessage)> UploadAsync(IFormFile image,string imageName , string folderPath, bool hasThumbnail)
        {
            var extension = Path.GetExtension(image.FileName);

            if (!_allowedExtensions.Contains(extension))
            {
                return (isUploded: false, errorMessage: ErrorValidation.NotAllowedExtensions);
            }

            if (image.Length > _maxAllowedSize)
            {
                return (isUploded: false, errorMessage: ErrorValidation.maxSize);
            }


            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{folderPath}", imageName);
            

            using var stream = File.Create(path);
            await image.CopyToAsync(stream);
            stream.Dispose();

            if (hasThumbnail)
            {
                var thumbPath = Path.Combine($"{_webHostEnvironment.WebRootPath}{folderPath}/thumb", imageName);
                using var Loadedimage = Image.Load(image.OpenReadStream());
                var ratio = (float)Loadedimage.Width / 200;
                var height = Loadedimage.Height / ratio;
                Loadedimage.Mutate(i => i.Resize(width: 200, height: (int)height));
                Loadedimage.Save(thumbPath);
            }
            return (isUploded: true, errorMessage: null);
            
        }
        #endregion
    }
}
