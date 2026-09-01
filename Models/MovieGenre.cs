using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMood.Models;

[Table("movie_genres")]
public class MovieGenre
{
    [Column("movie_id")]
    public int MovieId { get; set; }

    [Column("genre_id")]
    public int GenreId { get; set; }

    public Movie Movie { get; set; } = null!;
    public Genre Genre { get; set; } = null!;
}
