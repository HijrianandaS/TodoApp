using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebsiteTodo.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteTodo.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7103/api/Todo";

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var response = await _httpClient.GetAsync(_apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to retrieve data");
            }
            var jsonString = await response.Content.ReadAsStringAsync();
            return Content(jsonString, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> AddTodo([FromBody] Todo todo)
        {
            var content = new StringContent(JsonSerializer.Serialize(todo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to add todo");
            }
            return Ok(await response.Content.ReadAsStringAsync());
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTodo([FromBody] Todo todo)
        {
            var content = new StringContent(JsonSerializer.Serialize(todo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{todo.Id}", content);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to update todo");
            }
            return Ok(await response.Content.ReadAsStringAsync());
        }

    }
}
