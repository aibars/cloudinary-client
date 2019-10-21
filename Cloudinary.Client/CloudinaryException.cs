using System;

namespace Cloudinary.Client
{
    public class CloudinaryException : Exception
    {
        public CloudinaryException()
        {
        }

        public CloudinaryException(string message)
            : base(message)
        {
        }

        public CloudinaryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}