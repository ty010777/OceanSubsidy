using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net.Http;
using System.Text;

public class EmailApiHelper
{
    public static bool Send(string to, string subject, string body, string cc = "", string bcc = "")
    {
        var payload = JsonConvert.SerializeObject(new {
            To = to,
            Cc = cc,
            Bcc = bcc,
            Subject = subject,
            Body = body
        });

        using (var client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings["EmailAPI"]);

            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = client.SendAsync(request).GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            JObject res = JObject.Parse(result);

            if (res != null && bool.Parse(res["success"].ToString()))
            {
                return true;
            }
        }

        return false;
    }
}
