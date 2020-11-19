using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Utils.SberbankAcquiring.Models.Request;
using Utils.SberbankAcquiring.Models.Response;

namespace Utils.SberbankAcquiring
{
    public static class SberbankAPI
    {
        private static readonly Uri TestHost = new Uri("https://3dsec.sberbank.ru");
        private static readonly Uri ProductionHost = new Uri("https://securepayments.sberbank.ru");

        public static async Task<RegisterDOResponse> RegisterDO(RegisterDORequest request)
        {
            using (var client = new HttpClient())
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var content = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(request, serializerSettings));
                var uri = new Uri(TestHost, "payment/rest/register.do");
                var response = await client.PostAsync(uri, new FormUrlEncodedContent(content));
                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<RegisterDOResponse>(responseContent);
            }
        }
    }
}
