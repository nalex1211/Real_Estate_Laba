using Newtonsoft.Json.Linq;
using System.Xml;

namespace Real_Estate_Laba.Helpers;

public class Api_Service
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;

    public Api_Service(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task<string> GetPropertyData(string city)
    {
        var apiKey = _configuration.GetSection("ApiKey").Value;
        var apiHost = _configuration.GetSection("ApiHost").Value;
        var encoded = Uri.EscapeDataString(city);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://zillow56.p.rapidapi.com/search?location={encoded}"),
            Headers =
    {
        { "X-RapidAPI-Key", $"{apiKey}" },
        { "X-RapidAPI-Host", $"{apiHost}" },
    },
        };
        using (var response = await _client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var result = JToken.Parse(body).ToString((Newtonsoft.Json.Formatting)Formatting.Indented);
            return result;
        }
    }
}
