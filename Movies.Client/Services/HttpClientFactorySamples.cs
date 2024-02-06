using System.Net.Http.Headers;
using System.Text.Json;
using Movies.Client.Helpers;
using Movies.Client.Models;

namespace Movies.Client.Services;

public class HttpClientFactorySamples : IIntegrationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptionsWrapper _jsonSerializerOptionsWrapper;
    private readonly MoviesAPIClient _moviesAPIClient;

    public HttpClientFactorySamples(
        IHttpClientFactory httpClientFactory,
        JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper,
        MoviesAPIClient moviesAPIClient)
    {
        _httpClientFactory = httpClientFactory ??
            throw new ArgumentNullException(nameof(httpClientFactory));
        _jsonSerializerOptionsWrapper = jsonSerializerOptionsWrapper ??
            throw new ArgumentNullException(nameof(jsonSerializerOptionsWrapper));
        _moviesAPIClient = moviesAPIClient ??
            throw new ArgumentNullException(nameof(moviesAPIClient));
    }

    public async Task RunAsync()
    {
        //await GetFilmsAsync();
        //await GetMoviesWithTypesHttpClientAsync();
        await GetMoviesViaMoviesAPIClientAsync();
    }

    public async Task GetFilmsAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get, "api/films");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var movies = JsonSerializer.Deserialize<IEnumerable<Movie>>(
            content, _jsonSerializerOptionsWrapper.Options);
    }

    public async Task GetMoviesViaMoviesAPIClientAsync()
    {
        var movies = await _moviesAPIClient.GetMoviesAsync();
    }

    // public async Task GetMoviesWithTypesHttpClientAsync()
    // {
    //     var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
    //     request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //     var response = await _moviesAPIClient.Client.SendAsync(request);
    //     response.EnsureSuccessStatusCode();

    //     var content = await response.Content.ReadAsStringAsync();
    //     var movies = JsonSerializer.Deserialize<IEnumerable<Movie>>(
    //         content, _jsonSerializerOptionsWrapper.Options);
    // }

    // Not a good practice
    private async Task TestDisposeHttpClientAsync()
    {
        for (var i = 0; i < 10; i++)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    "https://www.google.com");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Request completed with status code " +
                    $"{response.StatusCode}");
            }
        }
    }

    // Not a good practice
    private async Task TestReuseHttpClientAsync()
    {
        var httpClient = new HttpClient();

        for (int i = 0; i < 10; i++)
        {
            var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://www.google.com");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Request completed with status code " +
                $"{response.StatusCode}");
        }
    }
}
