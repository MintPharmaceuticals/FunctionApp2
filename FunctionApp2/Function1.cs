using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionApp2.Models;
using System.Net;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;

namespace FunctionApp2 {
    public static class Function1 {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log) {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            string sessionId;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            log.LogInformation("1");

            string url = "http://vmsapdevcc01.mintpharmaceuticals.loca:50001/b1s/v1/Login";
            LoginRequest loginRequest = new LoginRequest() {
                Username = "manager",
                Password = "Spring24",
                CompanyDB = "MPI_PRODUCTION"
            };

            // Serialize request body to JSON and login
            string jsonRequestBody = JsonConvert.SerializeObject(loginRequest);
            log.LogInformation("2");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            log.LogInformation("3");
            using (var streamReader = new StreamReader(httpWebRequest.GetRequestStream())) {
                var result = streamReader.ReadToEnd();
                log.LogInformation("4");
                // Deserialize success response
                var responseInstance = JsonConvert.DeserializeObject<LoginResponse>(result);

                log.LogInformation("Logged in successfully.");
                

                sessionId = responseInstance.SessionId;
                log.LogInformation($"{sessionId}");
            }

            // Make the request for Get
            log.LogInformation("5");
            string url2 = "http://vmsapdevcc01.mintpharmaceuticals.loca:50001/b1s/v1/Invoices(133)";
            var httpWebRequest2 = (HttpWebRequest)WebRequest.Create(url2);
            httpWebRequest2.ContentType = "application/json";
            httpWebRequest2.Method = "GET";
            httpWebRequest2.Accept = "application/json";
            httpWebRequest2.Headers.Add("Cookie", $"B1SESSION={sessionId}");
            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest2.GetResponse()) {
                var result = "";
                using (var streamReader = new StreamReader(response.GetResponseStream())) {
                    result = streamReader.ReadToEnd();
                }
                if (response.StatusCode == HttpStatusCode.OK) {

                    JObject jsonObject = JObject.Parse(result);
                    var readDocEntry = (int)jsonObject["DocEntry"];
                    var readDocNum = (int)jsonObject["DocNum"];
                    var readCardCode = (string)jsonObject["CardCode"];
                    string jsonRes = string.Format("AR Invoice retrieved successfully with DocEntry = {0} and DocNum = {1} for CardCode = {2}", readDocEntry, readDocNum, readCardCode);
                    log.LogInformation(jsonRes);
                }
            }
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }

}
