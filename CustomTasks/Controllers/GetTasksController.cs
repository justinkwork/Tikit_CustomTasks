using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;

[ApiController]
[Route("api/tasks/{teamid}")]
public class GetTasksController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public GetTasksController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(string teamid)
    {
        var client = _httpClientFactory.CreateClient();
        var apiBase = _configuration["ThirdPartyApi:TaskUrl"];
        var apiKey = _configuration["ThirdPartyApi:ApiKey"];


        // 1) Look up tasks by team ID
        var tasksUrl = apiBase;
        var tasksRequest = new HttpRequestMessage(HttpMethod.Get, tasksUrl);
        

        var taskResponse = await client.SendAsync(tasksRequest);
        taskResponse.EnsureSuccessStatusCode();

        var taskJson = await taskResponse.Content.ReadAsStringAsync();
        Console.WriteLine(taskJson);
        var taskOdata = JsonSerializer.Deserialize<List<TaskDto>>(taskJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return Ok(taskOdata?.Where(t => t.TeamId == int.Parse(teamid)) ?? new List<TaskDto>());
    }
}

public class TaskDto
{
    public int ID { get; set; }
    public string? Title { get; set; }
    public DateTime Created { get; set; }
    public float TeamId { get; set; }
    public string? Url { get; set; }
    public MethodDto? Method { get; set; }
}
public class MethodDto
{
    public string? Value { get; set; }
}
