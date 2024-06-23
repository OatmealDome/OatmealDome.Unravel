namespace OatmealDome.Unravel.Framework.Request;

internal abstract class ThreadsFormRequest : ThreadsUrlEncodedContentRequest
{
    public override HttpMethod Method => HttpMethod.Get;

    public override HttpContent? CreateHttpContent()
    {
        return CreateFormUrlEncodedContent();
    }

    public override FormUrlEncodedContent? CreateQueryParameters()
    {
        return null;
    }
}