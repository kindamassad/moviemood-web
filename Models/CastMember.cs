using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMood.Models;

[Table("cast_members")]
public class CastMember
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("movie_id")]
    public int MovieId { get; set; }

    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("character_name")]
    [MaxLength(100)]
    public string CharacterName { get; set; } = string.Empty;

    [Column("photo_path")]
    [MaxLength(255)]
    public string? PhotoPath { get; set; }

    public Movie Movie { get; set; } = null!;

    [NotMapped]
    public string PhotoUrl => string.IsNullOrEmpty(PhotoPath) ? "/images/cast/default.svg" : PhotoPath;
}
