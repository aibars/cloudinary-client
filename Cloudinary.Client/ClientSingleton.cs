namespace Cloudinary.Client
{
    public class ClientSingleton
    {
        private static ClientSingleton _instance;
        public CloudinaryDotNet.Cloudinary Client;

        public ClientSingleton()
        {
        }

        public static ClientSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClientSingleton();
                }

                return _instance;
            }
        }
    }
}
