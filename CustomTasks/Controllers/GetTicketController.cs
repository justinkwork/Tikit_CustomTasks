using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;

[ApiController]
[Route("api/tickets/{id}")]
public class GetTicketController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public GetTicketController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetTicket(string id)
    {
        var client = _httpClientFactory.CreateClient();
        var apiBase = _configuration["ThirdPartyApi:BaseUrl"];
        var apiKey = _configuration["ThirdPartyApi:ApiKey"];

        // 1) Look up ticket by ID
        var ticketUrl = $"{apiBase}/Ticket({id})";
        var ticketRequest = new HttpRequestMessage(HttpMethod.Get, ticketUrl);
        ticketRequest.Headers.Add("Authorization", $"Bearer {apiKey}");

        var ticketResponse = await client.SendAsync(ticketRequest);
        ticketResponse.EnsureSuccessStatusCode();

        var ticketJson = await ticketResponse.Content.ReadAsStringAsync();
        var ticketOdata = JsonSerializer.Deserialize<TicketDto>(ticketJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return Ok(ticketOdata ?? new TicketDto());
    }
}

public class TicketDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedDate { get; set; }
    public int TeamId { get; set; }
}
