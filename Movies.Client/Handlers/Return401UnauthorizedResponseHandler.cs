
using System.Net;

namespace Movies.Client.Handlers;

public class Return401UnauthorizedResponseHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        return Task.FromResult(response);
    }
}