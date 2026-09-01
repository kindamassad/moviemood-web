using Microsoft.AspNetCore.Mvc;
using MovieMood.Services;
using MovieMood.ViewModels;

namespace MovieMood.Controllers;

public class SearchController : Controller
{
    private readonly MovieService _movieService;

    public SearchController(MovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<IActionResult> Index(
        string? query,
        int page = 1,
        int? year_from = null,
        int? year_to = null,
        int? genre = null,
        double? rating = null,
        string sort_by = "popularity.desc",
        bool discover = false)
    {
        var genres = await _movieService.GetGenresAsync();
        var hasFilters = year_from.HasValue || year_to.HasValue || genre.HasValue ||
                         rating.HasValue || sort_by != "popularity.desc";

        var results = new List<Models.Movie>();
        var totalResults = 0;
        var totalPages = 1;

        if (hasFilters || discover || !string.IsNullOrWhiteSpace(query))
        {
            (results, totalResults, totalPages) = await _movieService.SearchMoviesAsync(
                query, page, year_from, year_to, genre, rating, sort_by);
        }

        return View(new SearchIndexViewModel
        {
            Query = query ?? string.Empty,
            Page = page,
            YearFrom = year_from,
            YearTo = year_to,
            Genre = genre,
            Rating = rating,
            SortBy = sort_by,
            Discover = discover,
            Genres = genres,
            Results = results,
            TotalResults = totalResults,
            TotalPages = totalPages,
            StartPage = Math.Max(1, page - 2),
            EndPage = Math.Min(totalPages, page + 2)
        });
    }
}
