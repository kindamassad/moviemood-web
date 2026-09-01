using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMood.Models;

[Table("category_movies")]
public class CategoryMovie
{
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("movie_id")]
    public int MovieId { get; set; }

    [Column("display_order")]
    public int DisplayOrder { get; set; }

    public Category Category { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}
