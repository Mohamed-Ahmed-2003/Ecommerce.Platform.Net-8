using Microsoft.Extensions.Options;
using OtlobLap.Configuration;

namespace OtlobLap.Validations
{
    public class ImageValidator(IOptions<ProductImageUploadOptions> options)
    {
        private readonly ProductImageUploadOptions _options = options.Value;

        public bool IsValidImageCollection(int collectionNum)
        {
            if (collectionNum < 0)
            {
                throw new ArgumentNullException(nameof(collectionNum), "Image collection is null.");
            }
            if (collectionNum > _options.MaximumImages)
                throw new ArgumentException($"Images Number exeeds {_options.MaximumImages}.");

            return true;
        }

        public bool IsValidImage(IFormFile formFile)
        {
            if (formFile == null)
            {
                throw new ArgumentNullException(nameof(formFile), "The file is null.");
            }

            if ((formFile.Length / 1024) > _options.MaxImageSizeKB)
            {
                throw new ArgumentException($"Image size exceeds {_options.MaxImageSizeKB} KB.");
            }

            var fileExtension = Path.GetExtension(formFile.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !_options.AllowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Image format is not acceptable.");
            }

            return true;
        }
    }

}
