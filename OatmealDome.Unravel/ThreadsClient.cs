using System.Text;
using System.Text.Json;
using System.Web;
using OatmealDome.Unravel.Authentication;
using OatmealDome.Unravel.Framework.Request;
using OatmealDome.Unravel.Framework.Response;

namespace OatmealDome.Unravel;

public class ThreadsClient
{
    private const string ApiBaseUrl = "https://graph.threads.net/";
    
    private static readonly HttpClient SharedClient = new HttpClient();
    
    private readonly HttpClient _httpClient;

    private readonly ulong _clientId;
    private readonly string _clientSecret;

    public ThreadsCredentials? Credentials
    {
        get;
        set;
    }

    static ThreadsClient()
    {
        Version version = typeof(ThreadsClient).Assembly.GetName().Version!;

        SharedClient.DefaultRequestHeaders.Add("User-Agent",
            $"Unravel/{version.Major}.{version.Minor}.{version.Revision}");
    }

    public ThreadsClient(HttpClient httpClient, ulong clientId, string clientSecret)
    {
        _httpClient = httpClient;

        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    public ThreadsClient(ulong clientId, string clientSecret) : this(SharedClient, clientId, clientSecret)
    {
        //
    }
    
    //
    // Internals
    //

    private async Task<HttpResponseMessage> SendRequest(ThreadsRequest request)
    {
        StringBuilder urlBuilder = new StringBuilder();

        urlBuilder.Append(ApiBaseUrl);

        if (request.Endpoint[0] != '/')
        {
            urlBuilder.Append('/');
        }

        urlBuilder.Append(request.Endpoint);

        FormUrlEncodedContent? urlContent = request.CreateQueryParameters();

        if (urlContent != null)
        {
            urlBuilder.Append('?');
            urlBuilder.Append(await urlContent.ReadAsStringAsync());
        }
        
        if (request.AuthenticationType == ThreadsRequestAuthenticationType.Authenticated)
        {
            if (Credentials == null)
            {
                throw new ThreadsException("Must authenticate to use this API endpoint");
            }

            if (Credentials.Expiry < DateTime.UtcNow)
            {
                throw new ThreadsException("Credentials have expired");
            }

            if (urlContent == null)
            {
                urlBuilder.Append('?');
            }
            else
            {
                urlBuilder.Append('&');
            }
            
            urlBuilder.Append("access_token=");
            urlBuilder.Append(HttpUtility.UrlEncode(Credentials.AccessToken));
        }

        string url = urlBuilder.ToString();

        HttpRequestMessage requestMessage = new HttpRequestMessage(request.Method, url)
        {
            Content = request.CreateHttpContent()
        };
        
        HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);
        
        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new ThreadsException(await responseMessage.Content.ReadAsStringAsync());
        }

        return responseMessage;
    }

    private async Task<T> SendRequestWithJsonResponse<T>(ThreadsRequest request) where T : ThreadsJsonResponse
    {
        using HttpResponseMessage message = await SendRequest(request);

        string json = await message.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(json)!;
    }
}