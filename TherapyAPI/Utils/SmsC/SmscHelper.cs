using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace Utils.SmsC
{
    public static class SmscHelper
    {
        public static async Task<string> SendSms(string phone, string message)
        {
            var qb = new QueryBuilder();
            qb.Add("login", "tkslovo");
            qb.Add("psw", "Kpy4KWcc#En");
            qb.Add("phones", phone);
            qb.Add("mes", message);

            var baseUri = new Uri("https://smsc.ru/sys/send.php").GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped);
            var fullUri = baseUri + qb.ToQueryString();

            var client = new HttpClient();
            var response = await client.GetAsync(fullUri);


           return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetNumberInfo(string phone)
        {
            var qb = new QueryBuilder();
            qb.Add("get_operator", "1");
            qb.Add("login", "tkslovo");
            qb.Add("psw", "Kpy4KWcc#En");
            qb.Add("phone", phone);

            var baseUri = new Uri("https://smsc.ru/sys/info.php").GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped);
            var fullUri = baseUri + qb.ToQueryString();

            var client = new HttpClient();
            var response = await client.GetAsync(fullUri);


            return await response.Content.ReadAsStringAsync();
        }
    }
}
