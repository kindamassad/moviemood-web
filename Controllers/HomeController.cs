using Microsoft.AspNetCore.Mvc;
using MovieMood;
using MovieMood.Models;
using MovieMood.Services;
using MovieMood.ViewModels;

namespace MovieMood.Controllers;

public class HomeController : Controller
{
    private readonly MovieService _movieService;

    public HomeController(MovieService movieService)
    {
        _movieService = movieService;
    }
    public async Task<IActionResult> Index()
    {
        var moviesData = await _movieService.GetHomeCategoriesAsync();

        Movie? featuredMovie = null;
        foreach (var categoryMovies in moviesData.Values)
        {
            if (categoryMovies.Count > 0)
            {
                featuredMovie = categoryMovies[0];
                break;
            }
        }

        return View(new HomeIndexViewModel
        {
            CategoryTitles = AppConstants.HomeCategoryTitles,
            MoviesData = moviesData,
            FeaturedMovie = featuredMovie
        });
    }
}
