using OatmealDome.Unravel.Framework.Request;

namespace OatmealDome.Unravel.Authentication;

internal class LongLivedAccessTokenRequest : ThreadsQueryRequest
{
    public override string Endpoint => "/refresh_access_token";

    public override ThreadsRequestAuthenticationType AuthenticationType =>
        ThreadsRequestAuthenticationType.Authenticated;

    [ThreadsUrlEncodedParameterName("grant_type")]
    public string GrantType
    {
        get;
        private set;
    } = "th_refresh_token";
}