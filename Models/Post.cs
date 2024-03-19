namespace BlogEF3.Models;

public class Post
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
    public Category? Category { get; set; }
    public User? Author { get; set; }

    public List<Tag> Tags { get; set; } = new List<Tag>();
}