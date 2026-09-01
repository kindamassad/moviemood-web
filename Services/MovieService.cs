using Microsoft.EntityFrameworkCore;
using MovieMood.Data;
using MovieMood.Models;

namespace MovieMood.Services;

public class MovieService
{
    private readonly ApplicationDbContext _db;
    private const int PageSize = 20;

    public MovieService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Dictionary<string, List<Movie>>> GetHomeCategoriesAsync(int limit = 6)
    {
        var categories = await _db.Categories.OrderBy(c => c.Id).ToListAsync();
        var result = new Dictionary<string, List<Movie>>();

        foreach (var category in categories)
        {
            var categoryMovies = await _db.CategoryMovies
                .Where(cm => cm.CategoryId == category.Id)
                .OrderBy(cm => cm.DisplayOrder)
                .Take(limit)
                .Include(cm => cm.Movie)
                    .ThenInclude(m => m.Genre)
                .ToListAsync();

            result[category.Slug] = categoryMovies.Select(cm => cm.Movie).ToList();
        }

        return result;
    }

    public async Task<List<Category>> GetCategoriesAsync()
        => await _db.Categories.OrderBy(c => c.Id).ToListAsync();

    public async Task<List<Genre>> GetGenresAsync()
        => await _db.Genres.OrderBy(g => g.Name).ToListAsync();

    public async Task<Movie?> GetMovieDetailsAsync(int id)
        => await _db.Movies
            .Include(m => m.Genre)
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.CastMembers)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<List<Movie>> GetSimilarMoviesAsync(int id, int limit = 6)
    {
        var movie = await _db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null) return new List<Movie>();

        return await _db.Movies
            .Where(m => m.Id != id && m.GenreId == movie.GenreId)
            .OrderByDescending(m => m.VoteAverage)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<(List<Movie> Results, int TotalResults, int TotalPages)> SearchMoviesAsync(
        string? query,
        int page,
        int? yearFrom,
        int? yearTo,
        int? genreId,
        double? minRating,
        string sortBy)
    {
        page = Math.Max(1, page);
        var movies = _db.Movies.Include(m => m.Genre).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim().ToLower();
            movies = movies.Where(m =>
                m.Title.ToLower().Contains(term) ||
                m.Overview.ToLower().Contains(term) ||
                (m.Director != null && m.Director.ToLower().Contains(term)));
        }

        if (yearFrom.HasValue)
            movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value.Year >= yearFrom);

        if (yearTo.HasValue)
            movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value.Year <= yearTo);

        if (genreId.HasValue)
            movies = movies.Where(m => m.GenreId == genreId || m.MovieGenres.Any(mg => mg.GenreId == genreId));

        if (minRating.HasValue)
            movies = movies.Where(m => m.VoteAverage >= minRating);

        movies = sortBy switch
        {
            "vote_average.desc" => movies.OrderByDescending(m => m.VoteAverage),
            "vote_average.asc" => movies.OrderBy(m => m.VoteAverage),
            "release_date.desc" => movies.OrderByDescending(m => m.ReleaseDate),
            "release_date.asc" => movies.OrderBy(m => m.ReleaseDate),
            "original_title.asc" => movies.OrderBy(m => m.Title),
            "original_title.desc" => movies.OrderByDescending(m => m.Title),
            _ => movies.OrderByDescending(m => m.VoteAverage)
        };

        var total = await movies.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        var results = await movies.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

        return (results, total, totalPages);
    }

    public async Task<List<Movie>> GetMoviesByMoodAsync(IEnumerable<string> genreNames, int count = 3)
    {
        var names = genreNames.ToList();
        var movies = await _db.Movies
            .Include(m => m.Genre)
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Where(m => names.Contains(m.Genre.Name) ||
                        m.MovieGenres.Any(mg => names.Contains(mg.Genre.Name)))
            .OrderByDescending(m => m.VoteAverage)
            .ToListAsync();

        return movies.OrderBy(_ => Guid.NewGuid()).Take(Math.Min(count, movies.Count)).ToList();
    }

    public async Task<Dictionary<int, Movie>> GetMoviesByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids.Distinct().ToList();
        var movies = await _db.Movies
            .Include(m => m.Genre)
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Where(m => idList.Contains(m.Id))
            .ToListAsync();

        return movies.ToDictionary(m => m.Id);
    }
}
