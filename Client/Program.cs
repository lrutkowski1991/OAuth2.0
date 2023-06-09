// See https://aka.ms/new-console-template for more information
using System.Net.Http.Headers;
using System.Net.Http.Json;

var accessToken = await GetAccessToken();

if (!string.IsNullOrEmpty(accessToken))
{
    await AccessProtectedResource(accessToken);
}

Console.WriteLine("Press any key to exit.");
Console.ReadKey();


static async Task<string> GetAccessToken()
{
    using (var client = new HttpClient())
    {
        client.BaseAddress = new Uri("http://localhost:5191/"); // Authorization server address
    
        var tokenEndpoint = "api/Auth/GetAccessToken";
        var clientId = "client1";
        var clientSecret = "secret1";
        var username = "user1";
        var password = "password1";
        
        var tokenRequest = new TokenRequest
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            Username = username,
            Password = password
        };
        
        var response = await client.PostAsJsonAsync(tokenEndpoint, tokenRequest);
        var test = response.Content;
        Console.WriteLine(response.ToString());
        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
            Console.WriteLine(tokenResponse.ToString());
            return tokenResponse.AccessToken;
        }
        
        Console.WriteLine("Failed to obtain access token.");
        return null;
    }
}

static async Task AccessProtectedResource(string accessToken)
{
    using (var client = new HttpClient())
    {
        client.BaseAddress = new Uri("https://localhost:7294/"); // Resource server address

        var resourceEndpoint = "WeatherForecast";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync(resourceEndpoint);
        if (response.IsSuccessStatusCode)
        {
            var resources = await response.Content.ReadAsAsync<IEnumerable<WeatherForecast>>();
            Console.WriteLine("Protected Resources:");
            foreach (var resource in resources)
            {
                Console.WriteLine($"date: {resource.Date}, temperatureC: {resource.TemperatureC}, temperatureF: {resource.TemperatureF}, summary: {resource.Summary}");
            }
        }
        else
        {
            Console.WriteLine("Failed to access protected resources.");
        }
    }
    
}

// A class that represents an access token request
public class TokenRequest
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

// A class that represents a response with an access token
public class TokenResponse
{
    public string AccessToken { get; set; }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}