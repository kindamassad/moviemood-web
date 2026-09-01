using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMood.Models;

[Table("genres")]
public class Genre
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}
