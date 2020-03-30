namespace Library.ClassServices
{
    using Library.Classes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class APIService
    {
        public static async Task<Response> GetCountries(string baseUrl, string controller)
        {
            try
            {              
                var client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync(controller);

                var result = await response.Content.ReadAsStringAsync();

                //Se algo correr mal...
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,

                    };
                }
                var countries = JsonConvert.DeserializeObject<List<Country>>(result); //Converter Json numa lista de dados do tipo rate

                return new Response
                {
                    IsSuccess = true,
                    Result = countries,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
