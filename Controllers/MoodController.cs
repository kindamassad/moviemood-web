using Microsoft.AspNetCore.Mvc;
using MovieMood;
using MovieMood.Services;
using MovieMood.ViewModels;

namespace MovieMood.Controllers;

public class MoodController : Controller
{
    private readonly MovieService _movieService;

    public MoodController(MovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<IActionResult> Index(string? mood)
    {
        var model = new MoodIndexViewModel
        {
            Moods = AppConstants.Moods
        };

        if (!string.IsNullOrEmpty(mood) && AppConstants.Moods.TryGetValue(mood, out var moodData))
        {
            model.SelectedMood = mood;
            model.SelectedMoodData = moodData;
            model.SuggestedMovies = await _movieService.GetMoviesByMoodAsync(moodData.GenreNames);
        }

        return View(model);
    }
}
