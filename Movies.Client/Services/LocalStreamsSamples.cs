using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Movies.Client.Helpers;
using Movies.Client.Models;

namespace Movies.Client.Services;

public class LocalStreamsSamples : IIntegrationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptionsWrapper _jsonSerializerOptionsWrapper;

    public LocalStreamsSamples(
        IHttpClientFactory httpClientFactory, JsonSerializerOptionsWrapper jsonSerializerOptionsWrapper)
    {
        _httpClientFactory = httpClientFactory ??
            throw new ArgumentNullException(nameof(httpClientFactory));
        _jsonSerializerOptionsWrapper = jsonSerializerOptionsWrapper ??
            throw new ArgumentNullException(nameof(jsonSerializerOptionsWrapper));
    }

    public async Task RunAsync()
    {
        //await GetPosterWithStreamAsync();
        //await GetPosterWithStreamAndCompletionModeAsync();
        //await TestMethodAsync(() => GetPosterWithoutStreamAsync());
        //await TestMethodAsync(() => GetPosterWithStreamAsync());
        //await TestMethodAsync(() => GetPosterWithStreamAndCompletionModeAsync());
        //await PostPosterWithStreamAsync();
        //await PostAndReadPosterWithStreamsAsync();
        await TestMethodAsync(() => PostPosterWithoutStreamsAsync());
        await TestMethodAsync(() => PostPosterWithStreamAsync());
        await TestMethodAsync(() => PostAndReadPosterWithStreamsAsync());
    }

    public async Task GetPosterWithStreamAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using (var response = await httpClient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var poster = await JsonSerializer.DeserializeAsync<Poster>(
                stream, _jsonSerializerOptionsWrapper.Options);
        }
    }

    public async Task GetPosterWithStreamAndCompletionModeAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using (var response = await httpClient.SendAsync(
            request, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var poster = await JsonSerializer.DeserializeAsync<Poster>(
                stream, _jsonSerializerOptionsWrapper.Options);
        }
    }

    public async Task GetPosterWithoutStreamAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using (var response = await httpClient.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var poster = JsonSerializer.Deserialize<Poster>(
                content, _jsonSerializerOptionsWrapper.Options);
        }
    }

    public async Task PostPosterWithStreamAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var random = new Random();
        var generatedBytes = new byte[5242880];
        random.NextBytes(generatedBytes);

        var posterForCreation = new PosterForCreation()
        {
            Name = "A new poster for The Big Lebowski",
            Bytes = generatedBytes
        };

        using (var memoryContentStream = new MemoryStream())
        {
            await JsonSerializer.SerializeAsync(memoryContentStream, posterForCreation);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(
                HttpMethod.Post, "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var streamContent = new StreamContent(memoryContentStream))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = streamContent;

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var poster = JsonSerializer.Deserialize<Poster>(
                        content, _jsonSerializerOptionsWrapper.Options);
                }
            }
        }
    }

    public async Task PostAndReadPosterWithStreamsAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var random = new Random();
        var generatedBytes = new byte[5242880];
        random.NextBytes(generatedBytes);

        var posterForCreation = new PosterForCreation()
        {
            Name = "A new poster for The Big Lebowski",
            Bytes = generatedBytes
        };

        using (var memoryContentStream = new MemoryStream())
        {
            await JsonSerializer.SerializeAsync(memoryContentStream, posterForCreation);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(
                HttpMethod.Post, "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var streamContent = new StreamContent(memoryContentStream))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = streamContent;

                    using (var response = await httpClient.SendAsync(
                        request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        var stream = await response.Content.ReadAsStreamAsync();
                        var poster = await JsonSerializer.DeserializeAsync<Poster>(
                            stream, _jsonSerializerOptionsWrapper.Options);
                    }
                }
            }
        }
    }

    public async Task PostPosterWithoutStreamsAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("MoviesAPIClient");

        var random = new Random();
        var generatedBytes = new byte[5242880];
        random.NextBytes(generatedBytes);

        var posterForCreation = new PosterForCreation()
        {
            Name = "A new poster for The Big Lebowski",
            Bytes = generatedBytes
        };

        var serializedPosterToCreate = JsonSerializer.Serialize(
            posterForCreation, _jsonSerializerOptionsWrapper.Options);

        var request = new HttpRequestMessage(
            HttpMethod.Post, "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        request.Content = new StringContent(serializedPosterToCreate);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var poster = JsonSerializer.Deserialize<Poster>(
            content, _jsonSerializerOptionsWrapper.Options);
    }

    public async Task TestMethodAsync(Func<Task> functionToTest)
    {
        await functionToTest();

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 200; i++)
        {
            await functionToTest();
        }

        stopwatch.Stop();
        Console.WriteLine(
            $"Elapsed milliseconds without stream: {stopwatch.ElapsedMilliseconds}, averaging {stopwatch.ElapsedMilliseconds / 200} milliseconds/request");
    }
}
