namespace AspNetCoreSample.Mvc.Models;

public class VueIndexViewModel
{
    public string? UserName { get; set; }

    public string? Email { get; set; }

    public int Age { get; set; }

    public DateOnly Birthday { get; set; }

    public static VueIndexViewModel CreateSample() => new()
    {
        UserName = "UserName",
        Email = "Email",
        Age = 12,
        Birthday = new DateOnly(2000, 10, 1)
    };
}
