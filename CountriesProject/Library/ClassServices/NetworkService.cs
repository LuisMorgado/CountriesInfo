namespace Library.ClassServices
{
    using Library.Classes;
    using System.Net;

    public class NetworkService
    {

        /// <summary>
        /// Checks if exists network connections
        /// </summary>
        /// <returns>Response</returns>
        public Response CheckConnection()
        {
            var client = new WebClient();

            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return new Response
                    {
                        IsSuccess = true,
                    };
                }
            }
            catch
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Configure a sua ligação á internet",
                };
            }
        }
    }
}
