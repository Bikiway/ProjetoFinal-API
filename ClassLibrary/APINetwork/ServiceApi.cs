using Newtonsoft.Json;
using ProjetoFinal_API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClassLibrary.APINetwork
{
    public class ServiceApi
    {
        public async Task<Response> GetRates(string urlBase, string controller, IProgress<int> progress) 
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                //using var r = new StreamReader ("countries.json");
                //var result = r.ReadToEnd();

                progress.Report(25);
                var response = await client.GetAsync(controller, HttpCompletionOption.ResponseHeadersRead);

                progress.Report(50);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        Success = false,
                        Message = result,
                    };
                }

                progress.Report(75);
                var root = JsonConvert.DeserializeObject<ObservableCollection<Root>>(result);

                progress.Report(100);
                return new Response
                {
                    Success = true,
                    Result = root,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = ex.Message,
                };
            }

        }
    }
}
