using System.Net.Http;
using System.Threading.Tasks;

namespace ExampleApi.Services
{
    public interface IGitHubService
    {
        /// <summary>
        /// Retrieves the Top 5 Repos by stars from GitHub by the language provided.
        /// </summary>
        /// <param name="language"></param>
        /// <returns>Returns a HttpResponseMessage with the data in Content</returns>
        Task<HttpResponseMessage> GetTop5ReposAsync(string language);
    }
}
