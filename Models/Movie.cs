using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMood.Models;

[Table("movies")]
public class Movie
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Column("tagline")]
    [MaxLength(255)]
    public string? Tagline { get; set; }

    [Column("overview")]
    public string Overview { get; set; } = string.Empty;

    [Column("poster_path")]
    [MaxLength(255)]
    public string? PosterPath { get; set; }

    [Column("backdrop_path")]
    [MaxLength(255)]
    public string? BackdropPath { get; set; }

    [Column("release_date")]
    public DateTime? ReleaseDate { get; set; }

    [Column("vote_average")]
    public double VoteAverage { get; set; }

    [Column("runtime")]
    public int? Runtime { get; set; }

    [Column("original_language")]
    [MaxLength(10)]
    public string OriginalLanguage { get; set; } = "en";

    [Column("director")]
    [MaxLength(100)]
    public string? Director { get; set; }

    [Column("trailer_key")]
    [MaxLength(50)]
    public string? TrailerKey { get; set; }

    [Column("genre_id")]
    public int GenreId { get; set; }

    [ForeignKey(nameof(GenreId))]
    public Genre Genre { get; set; } = null!;

    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    public ICollection<CastMember> CastMembers { get; set; } = new List<CastMember>();
    public ICollection<CategoryMovie> CategoryMovies { get; set; } = new List<CategoryMovie>();

    [NotMapped]
    public string PosterUrl => string.IsNullOrEmpty(PosterPath) ? "/images/posters/default.svg" : PosterPath;

    [NotMapped]
    public string BackdropUrl => string.IsNullOrEmpty(BackdropPath) ? "/images/backdrops/default.svg" : BackdropPath;

    [NotMapped]
    public int? ReleaseYear => ReleaseDate?.Year;

    [NotMapped]
    public IEnumerable<Genre> AllGenres
    {
        get
        {
            var genres = new List<Genre>();
            if (Genre != null)
                genres.Add(Genre);
            genres.AddRange(MovieGenres.Select(mg => mg.Genre));
            return genres.GroupBy(g => g.Id).Select(g => g.First());
        }
    }
}
