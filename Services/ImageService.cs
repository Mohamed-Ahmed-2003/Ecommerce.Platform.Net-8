
using Microsoft.AspNetCore.Hosting;

namespace OtlobLap.Services
{
    public interface IImageService
    {
        public Task<string> UploadPhysical(IFormFile image, string folderName);
        public void RemoveImagePhysical(string folderName, string fileName);
        public Task<byte[]> UploadToDb(IFormFile file);
        public  IFormFile GetFormFileFromImagePath(string imagePath);
        
            
    }
    public class ImageService(IWebHostEnvironment webHost) : IImageService
    {
        private readonly IWebHostEnvironment _webHost = webHost;

        public  IFormFile GetFormFileFromImagePath(string imagePath)
        {
            imagePath = Path.Combine(_webHost.WebRootPath, "Images", "Users", "Admin.png");

            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Image file not found at path: {imagePath}");
            }

            var fileName = Path.GetFileName(imagePath);
            var contentType = GetContentType(fileName);

            var fileBytes = File.ReadAllBytes(imagePath);
            var fileStream = new MemoryStream(fileBytes);

            return new FormFile(fileStream, 0, fileBytes.Length, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        private static string GetContentType(string fileName)
        {
            // You may need to extend this method to properly determine the content type based on the file extension
            return "image/jpeg"; // Default content type for JPEG images
        }

        public void RemoveImagePhysical(string folderName , string fileName)
        {
            var path = Path.Combine(_webHost.WebRootPath, "Images", folderName, fileName);

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    throw new Exception("Failed to delete the image file", ex);
                }
            }
        }
        public async Task<string> UploadPhysical(IFormFile image,string folderName)
        {
            try
            {
            string wwwRootPath = _webHost.WebRootPath;
            var uploadDir = Path.Combine(wwwRootPath, "Images", folderName);
                if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            string fileName = Path.GetFileNameWithoutExtension(image.FileName);
            string extention = Path.GetExtension(image.FileName);
            var uniqueName = fileName + Guid.NewGuid().ToString() + extention;
            var toPath = Path.Combine(uploadDir, uniqueName);
            using (var file = new FileStream(toPath, FileMode.Create))
            {
                await image.CopyToAsync(file);
            }
            return uniqueName;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new Exception("Failed to upload the image to the server", ex);
            }
        }


        public async Task<byte[]> UploadToDb(IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        return stream.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    throw new Exception("Failed to upload the image to the database", ex);
                }
            }

            throw new ArgumentNullException(nameof(file), "File is null");
        }

     
    }
}
