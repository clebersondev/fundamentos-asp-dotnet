using System.Text.Json.Serialization;

namespace BlogEF3.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public IList<Post> Posts { get; set; } = new List<Post>();
    public IList<Role> Roles { get; set; } = new List<Role>();
}