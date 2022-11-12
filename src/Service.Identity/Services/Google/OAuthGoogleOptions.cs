using System;

#pragma warning disable CS8618

namespace Identity.Services.Google;

[Serializable]
public class OAuthGoogleOptions
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}