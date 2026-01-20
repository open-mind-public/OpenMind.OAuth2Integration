namespace OpenMind.CRM.Application.Exceptions;

/// <summary>
/// Exception thrown when OAuth token refresh fails and re-authorization is required.
/// </summary>
public class OAuthTokenExpiredException : Exception
{
    public string Provider { get; }
    
    public OAuthTokenExpiredException(string provider) 
        : base($"OAuth token for {provider} has expired and could not be refreshed. Re-authorization is required.")
    {
        Provider = provider;
    }
    
    public OAuthTokenExpiredException(string provider, string message) 
        : base(message)
    {
        Provider = provider;
    }
    
    public OAuthTokenExpiredException(string provider, string message, Exception innerException) 
        : base(message, innerException)
    {
        Provider = provider;
    }
}
