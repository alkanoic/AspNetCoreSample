
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class CreateUserRequest
{
    public required string Username { get; set; }
    public bool Enabled { get; set; } = true;
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required List<Credential> Credentials { get; set; }
    public class Credential
    {
        public required string Type { get; set; }
        public required string Value { get; set; }
        public bool Temporary { get; set; }
    }
}
