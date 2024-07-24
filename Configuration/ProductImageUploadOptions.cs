namespace OtlobLap.Configuration
{
    public class ProductImageUploadOptions
    {
             public int MaximumImages { get; set; }
            public List<string> AllowedExtensions { get; set; }
            public int MaxImageSizeKB { get; set; }

    }
}
