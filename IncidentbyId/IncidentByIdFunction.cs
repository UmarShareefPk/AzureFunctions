using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace IncidentbyId
{
    public static class IncidentByIdFunction
    {
        [FunctionName("IncidentById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string username = req.Query["user"];
            string password = req.Query["pass"];
            string Id = string.IsNullOrEmpty(req.Query["id"]) ? "A4A33EB4-FFCB-4742-B373-DB8640D3FE9C" : req.Query["id"];
            
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new BadRequestResult();
            }

            string token = await GetToken(username, password);

            if (string.IsNullOrEmpty(token))
            {
                return new OkObjectResult("Request recieved but without Token"); 
            }

            string baseUrl = "https://imwebapicore.azurewebsites.net/";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var response = await client.GetAsync(baseUrl + "api/Incidents/IncidentById?Id=" + Id);
                var result = await response.Content.ReadAsStringAsync();
                dynamic incidentData = JsonConvert.DeserializeObject(result);
                return new OkObjectResult(incidentData);
            }

            
            
        }

        private static async Task<string> GetToken(string username, string password)
        {
            string baseUrl = "https://imwebapicore.azurewebsites.net/";
            var url = baseUrl + "api/Users/authenticate";

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(new { username = username, password = password });
                var requestData = new StringContent(json, Encoding.UTF8, "application/json");
               

                var response = await client.PostAsync(url, requestData);

                var result = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(result);

                return data.Token.ToString();
            }
        }


    } // end of class
}
