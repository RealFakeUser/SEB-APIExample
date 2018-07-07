using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApi.Services
{
    public class GitHubService : IGitHubService
    {
        private const string ITEMS_PATH = "items";
        private readonly HttpClient _httpClient;
        private readonly string Host;

        public GitHubService()
        {
            Host = "api.github.com";

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/vnd.github.v3+json"));

            // Required by Github
            _httpClient.DefaultRequestHeaders.Add(name: "User-Agent", value: "Totally-Awesome-SEB-Demo");

        }

        public async Task<HttpResponseMessage> GetTop5ReposAsync(string language)
        {
            var builder = new UriBuilder(schemeName: "https", hostName: Host)
            {
                Path = "search/repositories",
                Query = $"q=language:{language}&sort=stars&order=desc"
            };

            var ghResponse = await SendGetRequest(builder.Uri);
            if (!ghResponse.IsSuccessStatusCode)
            {
                return ghResponse;
            }

            try
            {
                var ghResponseBody = await ghResponse.Content.ReadAsStringAsync();
                var reposList = JObject.Parse(ghResponseBody)[ITEMS_PATH].Select(i => i).ToList();
                var topFiveList = reposList.Count >= 5 ? reposList.Take(5).ToList() : reposList;
                var content = JsonConvert.SerializeObject(topFiveList);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "Error converting response data."};
            }
        }

        private async Task<HttpResponseMessage> SendGetRequest(Uri uri)
        {
            try
            {
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException reqEx)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = reqEx.Message
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
