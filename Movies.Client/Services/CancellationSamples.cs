using System.Net.Http.Headers;
using System.Text.Json;
using Movies.Client.Helpers;
using Movies.Client.Models;

namespace Movies.Client.Services;

public class CancellationSamples : IIntegrationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptionsWrapper _jsonSerializerOptionsWrapper;
    private CancellationTokenSource _cancellationTokenSource = new();

    public CancellationSamples(
        IHttpClientFactory httpClientFactory, JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
    {
        _httpClientFactory = httpClientFactory ??
            throw new ArgumentNullException(nameof(httpClientFactory));
        _jsonSerializerOptionsWrapper = jsonSerializerOptionsWrapper ??
            throw new ArgumentNullException(nameof(jsonSerializerOptionsWrapper));
    }

    public async Task RunAsync()
    {
        _cancellationTokenSource.CancelAfter(200);
        //await GetTrailerAndCancelAsync(_cancellationTokenSource.Token);
        await GetTrailerAndHandleTimeoutAsync();
    }

    public async Task GetTrailerAndCancelAsync(CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        try
        {
            using (var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var poster = await JsonSerializer.DeserializeAsync<Trailer>(
                    stream, _jsonSerializerOptionsWrapper.Options);
            }
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine($"An operation was cancelled with message {ex.Message}");
        }
    }

    public async Task GetTrailerAndHandleTimeoutAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        try
        {
            using (var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var poster = await JsonSerializer.DeserializeAsync<Trailer>(
                    stream, _jsonSerializerOptionsWrapper.Options);
            }
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine($"An operation was cancelled with message {ex.Message}");
        }
    }
}
