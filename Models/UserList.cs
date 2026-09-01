using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMood.Models;

[Table("user_lists")]
public class UserList
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("movie_id")]
    public int MovieId { get; set; }

    [Column("list_type")]
    [MaxLength(20)]
    public string ListType { get; set; } = string.Empty;

    [Column("list_name")]
    [MaxLength(50)]
    public string? ListName { get; set; }

    [Column("added_at")]
    public DateTime AddedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
