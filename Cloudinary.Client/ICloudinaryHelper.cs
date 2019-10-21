using CloudinaryDotNet.Actions;

namespace Cloudinary.Client
{
    public interface ICloudinaryHelper
    {
        string GetByPublicId(string id);

        ImageUploadResult UploadImage(string fileName, byte[] file);

        int GetPdfNumberOfPages(string url);

        ExplicitResult CreateCompressedImage(string publicId);
    }
}