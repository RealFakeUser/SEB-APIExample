using ExampleApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExampleApi.Controllers
{
    [Produces("application/json")]
    [Route("api/PopularRepos")]
    public class PopularReposController : Controller
    {
        private readonly IGitHubService _gitHubService;

        public PopularReposController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        // GET api/popularrepos/c
        [HttpGet("{language}")]
        public async Task<IActionResult> Get(string language)
        {
            var resp = await _gitHubService.GetTop5ReposAsync(language);
            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            else
            {
                return new StatusCodeResult((int)resp.StatusCode);
            }
        }
    }
}