namespace AspNetCoreSample.Mvc.Options;

public class VapidOption
{

    public const string Position = "Vapid";

    public string? PublicKey { get; set; }

    public string? PrivateKey { get; set; }
}
