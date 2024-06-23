using OatmealDome.Unravel.Authentication;

namespace OatmealDome.Unravel;

public class ThreadsClient
{
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
}