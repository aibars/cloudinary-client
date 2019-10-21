using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;

namespace Cloudinary.Client
{
    public class CloudinaryHelper : ICloudinaryHelper
    {
        private readonly IOptions<CloudinaryOptions> _configuration;
        public CloudinaryHelper(IOptions<CloudinaryOptions> configuration)
        {
            _configuration = configuration;
        }

        private CloudinaryDotNet.Cloudinary GetAccount()
        {
            if (ClientSingleton.Instance.Client == null)
            {
                var cloudinary = new CloudinaryDotNet.Cloudinary(new Account(
                    _configuration.Value.CloudName,
                    _configuration.Value.ApiKey,
                    _configuration.Value.ApiSecret));

                ClientSingleton.Instance.Client = cloudinary;

                return cloudinary;
            }
            else
            {
                return ClientSingleton.Instance.Client;
            }
        }

        /// <summary>
        /// Uploads an image in the compressed format
        /// </summary>
        public ImageUploadResult UploadImage(string fileName, byte[] file)
        {
            var account = GetAccount();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, new MemoryStream(file)),
                EagerTransforms = new List<Transformation>()
                {
                    new Transformation().Quality("auto:best")
                }
            };

            if (!string.IsNullOrEmpty(_configuration.Value.UploadFolder))
                uploadParams.Folder = _configuration.Value.UploadFolder;

            ImageUploadResult result = account.Upload(uploadParams);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                dynamic image = new ExpandoObject();
                image.PublicId = string.Format("{0}.{1}", result.PublicId, result.Format);
                image.PublicUri = result.Uri.ToString();
            }
            else
            {
                new CloudinaryException(result.Error.Message);
            }
            return result;
        }

        /// <summary>
        /// Gets the resource by the public Id
        /// </summary>
        public string GetByPublicId(string id)
        {
            var account = GetAccount();
            var url = account.Api.Url.Action("authenticated").ResourceType("image").Secure(true).Signed(true).BuildUrl(id);
            return url;
        }

        /// <summary>
        /// Applies the auto:best transform of an uploaded image
        /// </summary>
        public ExplicitResult CreateCompressedImage(string publicId)
        {
            var account = GetAccount();
            var exp = new ExplicitParams(publicId)
            {
                EagerTransforms = new List<Transformation>()
                {
                    new Transformation().Quality("auto:best")
                }
            };
            var expResult = account.Explicit(exp);

            if (expResult.StatusCode != HttpStatusCode.OK)
            {
                new CloudinaryException(expResult.Error.Message);
            }

            return expResult;
        }

        /// <summary>
        /// Given any PDF URL it will count the number of pages by uploading it to Cloudinary
        /// </summary>
        /// <param name="url">A valid URL of a PDF document</param>
        /// <returns>The number of pages</returns>
        public int GetPdfNumberOfPages(string url)
        {
            var cloudinary = GetAccount();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(url)
            };

            var uploadResult = cloudinary.Upload(uploadParams);

            if (uploadResult.StatusCode != HttpStatusCode.OK)
            {
                throw new CloudinaryException("Could not count number of pages of the selected PDF.");
            }

            int pages = uploadResult.Pages;

            cloudinary.DeleteResources(new DelResParams { PublicIds = new List<string> { uploadResult.PublicId } });

            return pages;
        }
    }
}