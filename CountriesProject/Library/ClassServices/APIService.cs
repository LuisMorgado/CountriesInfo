namespace Library.ClassServices
{
    using Library.Classes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class APIService
    {
        /// <summary>
        /// API connection
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="controller"></param>
        /// <returns>Countries</returns>
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

                var countries = JsonConvert.DeserializeObject<List<Country>>(result); //Converter Json numa lista de dados do tipo country
                FlagsDownloadAPI(countries);

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


        /// <summary>
        /// get flags from API
        /// </summary>
        /// <param name="countries"></param>
        public static void FlagsDownloadAPI(List<Country> countries)
        {
            WebClient client = new WebClient();

            if (!Directory.Exists("Data\\Imgs"))
            {
                Directory.CreateDirectory("Data\\imgs");
            }

            foreach (var country in countries)
            {
                client.DownloadFile(new Uri(country.Flag.AbsoluteUri), $@"Data\Imgs\{country.Alpha2Code}.svg");
            }
        }
    }
}
