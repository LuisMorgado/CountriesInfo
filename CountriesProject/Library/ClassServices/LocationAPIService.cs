namespace Library.ClassServices
{
    using Library.Classes;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// API connection
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="controller"></param>
    /// <returns>Countries</returns>
    public class LocationAPIService
    {
        public static async Task<Response> GetLocation(string baseUrl, string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }
                var locationResponse = JsonConvert.DeserializeObject<LocationResponse>(result);

                return new Response
                {
                    IsSuccess = true,
                    Result = locationResponse,
                };
            }
            catch (Exception e)
            {
                return new Response
                {
                    IsSuccess = false,
                    Result = e.Message,
                };
            }
        }  
    }
}
