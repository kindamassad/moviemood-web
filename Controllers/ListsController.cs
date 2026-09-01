using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMood.Data;
using MovieMood.Filters;
using MovieMood.Models;
using MovieMood.Services;
using MovieMood.ViewModels;

namespace MovieMood.Controllers;

public class ListsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly MovieService _movieService;

    public ListsController(ApplicationDbContext db, MovieService movieService)
    {
        _db = db;
        _movieService = movieService;
    }

    [RequireLogin]
    public async Task<IActionResult> Index(
        string list_type,
        string? list_name = null,
        int page = 1,
        int? year_from = null,
        int? year_to = null,
        int? genre = null,
        double? rating = null,
        string sort_by = "popularity.desc")
    {
        if (string.IsNullOrEmpty(list_type))
            return RedirectToAction("Index", "Account");

        var userId = HttpContext.Session.GetRequiredUserId();
        List<int> movieIds;

        if (list_type == "custom" && !string.IsNullOrEmpty(list_name))
        {
            movieIds = await _db.UserLists
                .Where(ul => ul.UserId == userId && ul.ListType == "custom" && ul.ListName == list_name)
                .Select(ul => ul.MovieId)
                .ToListAsync();
        }
        else
        {
            movieIds = await _db.UserLists
                .Where(ul => ul.UserId == userId && ul.ListType == list_type)
                .Select(ul => ul.MovieId)
                .ToListAsync();
        }

        var genres = await _movieService.GetGenresAsync();
        var moviesDict = await _movieService.GetMoviesByIdsAsync(movieIds);
        var filtered = FilterMovies(moviesDict, year_from, year_to, genre, rating);
        var sorted = SortMovies(filtered, sort_by);

        page = Math.Max(1, page);
        const int perPage = 20;
        var totalResults = sorted.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalResults / (double)perPage));
        var paged = sorted.Skip((page - 1) * perPage).Take(perPage).ToList();

        var title = list_type switch
        {
            "watchlist" => "Watchlist",
            "favorites" => "Favorites",
            "custom" => list_name ?? "Custom List",
            _ => "My List"
        };

        return View(new ListViewModel
        {
            ListType = list_type,
            ListName = list_name,
            Title = title,
            Page = page,
            YearFrom = year_from,
            YearTo = year_to,
            Genre = genre,
            Rating = rating,
            SortBy = sort_by,
            Genres = genres,
            Movies = paged,
            TotalResults = totalResults,
            TotalPages = totalPages,
            StartPage = Math.Max(1, page - 2),
            EndPage = Math.Min(totalPages, page + 2)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckMovie(int movieId)
    {
        if (!HttpContext.Session.IsLoggedIn() || movieId <= 0)
        {
            return Json(new
            {
                in_watchlist = false,
                in_favorites = false,
                in_custom = false,
                custom_lists = Array.Empty<string>()
            });
        }

        var userId = HttpContext.Session.GetRequiredUserId();

        var inWatchlist = await _db.UserLists.AnyAsync(ul =>
            ul.UserId == userId && ul.MovieId == movieId && ul.ListType == "watchlist");

        var inFavorites = await _db.UserLists.AnyAsync(ul =>
            ul.UserId == userId && ul.MovieId == movieId && ul.ListType == "favorites");

        var customLists = await _db.UserLists
            .Where(ul => ul.UserId == userId && ul.MovieId == movieId && ul.ListType == "custom")
            .Select(ul => ul.ListName!)
            .ToListAsync();

        return Json(new
        {
            in_watchlist = inWatchlist,
            in_favorites = inFavorites,
            in_custom = customLists.Count > 0,
            custom_lists = customLists
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleMovie(int movieId, string listType, string? listName = null)
    {
        if (!HttpContext.Session.IsLoggedIn())
            return Json(new { success = false, message = "User not authenticated" });

        if (movieId <= 0 || string.IsNullOrEmpty(listType))
            return Json(new { success = false, message = "Invalid data" });

        if (listType == "custom" && string.IsNullOrWhiteSpace(listName))
            return Json(new { success = false, message = "List name is required" });

        var userId = HttpContext.Session.GetRequiredUserId();
        listName = listName?.Trim();

        var query = _db.UserLists.Where(ul =>
            ul.UserId == userId &&
            ul.MovieId == movieId &&
            ul.ListType == listType);

        if (listType == "custom")
            query = query.Where(ul => ul.ListName == listName);

        var existing = await query.FirstOrDefaultAsync();

        if (existing != null)
        {
            _db.UserLists.Remove(existing);
            await _db.SaveChangesAsync();
            var label = listName ?? listType;
            return Json(new { success = true, message = $"Movie removed from \"{label}\"", action = "removed" });
        }

        if (listType == "custom")
        {
            var listExists = await _db.UserLists.AnyAsync(ul =>
                ul.UserId == userId && ul.ListType == "custom" && ul.ListName == listName);

            _db.UserLists.Add(new UserList
            {
                UserId = userId,
                MovieId = movieId,
                ListType = "custom",
                ListName = listName,
                AddedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            var message = listExists
                ? $"Movie added to existing list \"{listName}\""
                : $"New list \"{listName}\" created with this movie";

            return Json(new { success = true, message, action = "added" });
        }

        _db.UserLists.Add(new UserList
        {
            UserId = userId,
            MovieId = movieId,
            ListType = listType,
            AddedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = $"Movie added to \"{listType}\"", action = "added" });
    }

    [HttpGet]
    [RequireLogin]
    public async Task<IActionResult> GetLists()
    {
        var userId = HttpContext.Session.GetRequiredUserId();
        var lists = await _db.UserLists
            .Where(ul => ul.UserId == userId && ul.ListType == "custom")
            .Select(ul => ul.ListName!)
            .Distinct()
            .ToListAsync();

        return Json(new { success = true, lists });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireLogin]
    public async Task<IActionResult> DeleteList(string listName)
    {
        if (string.IsNullOrWhiteSpace(listName))
            return Json(new { success = false, message = "List name is required" });

        var userId = HttpContext.Session.GetRequiredUserId();
        listName = listName.Trim();

        var listExists = await _db.UserLists.AnyAsync(ul =>
            ul.UserId == userId && ul.ListType == "custom" && ul.ListName == listName);

        if (!listExists)
            return Json(new { success = false, message = "List not found" });

        var items = await _db.UserLists
            .Where(ul => ul.UserId == userId && ul.ListType == "custom" && ul.ListName == listName)
            .ToListAsync();

        _db.UserLists.RemoveRange(items);
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = $"List \"{listName}\" deleted successfully" });
    }

    private static Dictionary<int, Movie> FilterMovies(
        Dictionary<int, Movie> movies,
        int? yearFrom, int? yearTo, int? genre, double? rating)
    {
        var result = new Dictionary<int, Movie>();

        foreach (var (id, movie) in movies)
        {
            var include = true;

            if (yearFrom.HasValue && (!movie.ReleaseYear.HasValue || movie.ReleaseYear < yearFrom))
                include = false;
            if (yearTo.HasValue && (!movie.ReleaseYear.HasValue || movie.ReleaseYear > yearTo))
                include = false;
            if (genre.HasValue && include && movie.GenreId != genre && !movie.MovieGenres.Any(mg => mg.GenreId == genre))
                include = false;
            if (rating.HasValue && include && movie.VoteAverage < rating)
                include = false;

            if (include)
                result[id] = movie;
        }

        return result;
    }

    private static List<Movie> SortMovies(Dictionary<int, Movie> movies, string sortBy)
    {
        return sortBy switch
        {
            "vote_average.desc" => movies.Values.OrderByDescending(m => m.VoteAverage).ToList(),
            "vote_average.asc" => movies.Values.OrderBy(m => m.VoteAverage).ToList(),
            "release_date.desc" => movies.Values.OrderByDescending(m => m.ReleaseDate).ToList(),
            "release_date.asc" => movies.Values.OrderBy(m => m.ReleaseDate).ToList(),
            "original_title.asc" => movies.Values.OrderBy(m => m.Title).ToList(),
            "original_title.desc" => movies.Values.OrderByDescending(m => m.Title).ToList(),
            _ => movies.Values.OrderByDescending(m => m.VoteAverage).ToList()
        };
    }
}
