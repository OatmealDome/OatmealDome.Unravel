# OatmealDome.Threads

Unravel the Threads.

This is a .NET 6 library intended for bots that interact with Threads.

## Usage

At this time, it is only possible to log in and create posts.

First, [create a Meta app in the developer dashboard with the Threads use case](https://developers.facebook.com/apps).

You can then make a `ThreadsClient` instance:

```csharp
ThreadsClient client = new ThreadsClient("clientId", "clientSecret");
```

### Session Management

To interact with most APIs, you need to first obtain an access token.

If you need continued access to a user's account, save the `ThreadsCredentials` instance returned from an access token request. You can then set the `client.Credentials` property to reload the credentials at a later time.

#### Generating a Short-Lived Access Token

To obtain a short-lived access token, you first need to follow the authorization flow as described in the [Get Access Tokens and Permissions](https://developers.facebook.com/docs/threads/get-started/get-access-tokens-and-permissions) guide in the Threads API documentation.

You can generate an Authorization Window URL by using `Auth_GetUserOAuthAuthorizationUrl()`:

```csharp
// The URI that Threads should redirect to after the user gives authorization to your app.
string redirectUri = "http://example.com/redirect-uri";

string authorizationUrl = client.Auth_GetUserOAuthAuthorizationUrl(redirectUri, ThreadsPermission.Basic | ThreadsPermission.ContentPublish);
```

Check the `ThreadsPermission` enum for all possible permissions that you can request. Note that you *must* request the `Basic` permission at minimum.

Once you have an authorization code, you can then request a short-lived access token.

```csharp
ThreadsCredentials shortLivedCredentials = await client.Auth_GetShortLivedAccessToken(code, redirectUri);
```

This type of access token is valid for 1 hour.

#### Generating a Long-Lived Access Token

You can exchange a short-lived access token for a long-lived token. Long-lived tokens are valid for 60 days.

```csharp
ThreadsCredentials longLivedCredentials = await client.Auth_GetLongLivedAccessToken();
```

You can also generate a long-lived token manually by going to the "Customize" page for the Threads use case on your app. The generator can be found on the Settings tab. You may need to register the target user as a tester first.

#### Refreshing a Long-Lived Access Token

You can refresh a long-lived token before it expires.

```csharp
longLivedCredentials = await client.Auth_RefreshLongLivedAccessToken();
```

### Posting

To create a post, you must first create a media container.

#### Creating a Text Container

Text containers are only able to contain text.

```csharp
string containerId = await client.Publishing_CreateTextMediaContainer("Hello, World!");
```

#### Creating an Image Container

Image containers can contain both an image and some optional accompanying text.

```csharp
string containerId = await client.Publishing_CreateImageMediaContainer("http://example.com/image.jpg", "Optional text for this container");
```

#### Creating a Carousel Container

Carousel containers consist of up to 10 Image containers.

```csharp
List<string> subContainerIds = new List<string>();

foreach (string url in urls)
{
    string subContainerId = await client.Publishing_CreateImageMediaContainer(url, isCarouselItem: true);

    subContainerIds.Add(subContainerId);
}

string containerId = await client.Publishing_CreateCarouselMediaContainer(subContainerIds, "Optional text for this container");
```

#### Publishing a Container

Once a container has been created, it can be published. (If you are publishing an Image or Carousel container, wait at least 30 seconds after creating the container before attempting to publish it. The Threads system requires some time to process uploads.)

```csharp
await client.Publishing_PublishMediaContainer(containerId);
```
