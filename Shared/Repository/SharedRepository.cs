using Newtonsoft.Json;
using Shared.Repository.Interface;
using System.Text;

namespace Shared.Repository
{
    public class SharedRepository : IShared
    {
        public async Task<T> CallAsync<T>(string url, object objectToPost = null, Method httpMethod = Method.Get)
        {
            string response = string.Empty;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
            switch (httpMethod)
            {
                case Method.Get:
                    response = await (await client.GetAsync(url)).Content.ReadAsStringAsync();
                    break;
                case Method.Post:
                    response = await (await client.PostAsync(url,
                    new StringContent(JsonConvert.SerializeObject(objectToPost), Encoding.UTF8, "application/json"))).Content.ReadAsStringAsync();
                    break;
                default:
                    break;
            }
            return JsonConvert.DeserializeObject<T>(response);
        }
    }
}
