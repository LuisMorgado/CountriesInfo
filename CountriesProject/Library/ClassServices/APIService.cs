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

    public class APIService
    {
        DialogService dialogService = new DialogService();
        
        /// <summary>
        /// API connection
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="controller"></param>
        /// <returns>Countries</returns>
        public async Task<Response> GetCountries(string baseUrl, string controller, IProgress<ProgressReport> progress)
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

                var countries = JsonConvert.DeserializeObject<List<Country>>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }); //Converter Json numa lista de dados do tipo country


                await FlagsDownloadAPI(countries, progress);

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
        public async Task FlagsDownloadAPI(List<Country> countries, IProgress<ProgressReport> progress)
        {
            WebClient client = new WebClient();
            ProgressReport progressReport = new ProgressReport();
            byte p = 1;

            await Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists("Data\\Imgs"))
                    {
                        Directory.CreateDirectory("Data\\imgs");
                    }

                    foreach (var country in countries)
                    {
                        client.DownloadFile(new Uri(country.Flag.AbsoluteUri), $@"Data\Imgs\{country.Alpha2Code}.svg");

                        progressReport.Percentage = Convert.ToByte(((p * 100) / countries.Count));
                        progress.Report(progressReport);
                        p++;
                    }

                    client.DownloadFile("https://upload.wikimedia.org/wikipedia/commons/b/b0/No_flag.svg", $@"Data\Imgs\Default.svg");
                    client.Dispose(); //clean up webCLient from memory
                }
                catch (Exception)
                {
                    dialogService.ShowMessage("Error", "Flag not found");
                }
            });


        }
    }
}
