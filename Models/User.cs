using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMood.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Column("email")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Column("password")]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public ICollection<UserList> UserLists { get; set; } = new List<UserList>();
}
