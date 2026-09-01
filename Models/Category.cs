using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMood.Models;

[Table("categories")]
public class Category
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("slug")]
    [MaxLength(50)]
    public string Slug { get; set; } = string.Empty;

    [Column("title")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<CategoryMovie> CategoryMovies { get; set; } = new List<CategoryMovie>();
}
