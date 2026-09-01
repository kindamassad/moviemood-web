using Microsoft.AspNetCore.Mvc;
using MovieMood.Services;
using MovieMood.ViewModels;

namespace MovieMood.Controllers;

public class MoviesController : Controller
{
    private readonly MovieService _movieService;

    public MoviesController(MovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<IActionResult> Details(int id, bool playTrailer = false)
    {
        var movie = await _movieService.GetMovieDetailsAsync(id);
        if (movie == null)
            return RedirectToAction("Index", "Home");

        var similarMovies = await _movieService.GetSimilarMoviesAsync(id);

        return View(new MovieDetailsViewModel
        {
            Movie = movie,
            SimilarMovies = similarMovies,
            PlayTrailer = playTrailer
        });
    }
}
