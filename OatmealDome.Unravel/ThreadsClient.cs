namespace OatmealDome.Unravel;

public class ThreadsClient
{
    private static readonly HttpClient SharedClient = new HttpClient();
    
    private readonly HttpClient _httpClient;

    static ThreadsClient()
    {
        Version version = typeof(ThreadsClient).Assembly.GetName().Version!;

        SharedClient.DefaultRequestHeaders.Add("User-Agent",
            $"Unravel/{version.Major}.{version.Minor}.{version.Revision}");
    }

    public ThreadsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public ThreadsClient() : this(SharedClient)
    {
        //
    }
}