namespace WebApiCore.Secutiry;

public class JwkOptions
{
    public string Issuer { get; private set; }

    public Uri JwksUri { get; private set; }

    public TimeSpan KeepFor { get; private set; }
    public JwkOptions(string? jwksUri)
    {
        if (string.IsNullOrWhiteSpace(jwksUri))
            throw new ArgumentNullException(nameof(jwksUri), "JwksUri não pode ser nulo ou vazio.");
        JwksUri = new Uri(jwksUri);
        Issuer = JwksUri.Scheme + "://" + JwksUri.Authority;
        KeepFor = TimeSpan.FromMinutes(15.0);
    }
}
