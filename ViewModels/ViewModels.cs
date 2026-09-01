using System.ComponentModel.DataAnnotations;
using MovieMood.Models;

namespace MovieMood.ViewModels;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class ChangePasswordViewModel
{
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

public class HomeIndexViewModel
{
    public Dictionary<string, string> CategoryTitles { get; set; } = new();
    public Dictionary<string, List<Movie>> MoviesData { get; set; } = new();
    public Movie? FeaturedMovie { get; set; }
}

public class SearchIndexViewModel
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public int? Genre { get; set; }
    public double? Rating { get; set; }
    public string SortBy { get; set; } = "popularity.desc";
    public bool Discover { get; set; }
    public List<Genre> Genres { get; set; } = new();
    public List<Movie> Results { get; set; } = new();
    public int TotalResults { get; set; }
    public int TotalPages { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
}

public class MovieDetailsViewModel
{
    public Movie Movie { get; set; } = null!;
    public List<Movie> SimilarMovies { get; set; } = new();
    public bool PlayTrailer { get; set; }
}

public class MoodIndexViewModel
{
    public Dictionary<string, MoodConfig> Moods { get; set; } = new();
    public string? SelectedMood { get; set; }
    public MoodConfig? SelectedMoodData { get; set; }
    public List<Movie> SuggestedMovies { get; set; } = new();
}

public class ListViewModel
{
    public string ListType { get; set; } = string.Empty;
    public string? ListName { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Page { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public int? Genre { get; set; }
    public double? Rating { get; set; }
    public string SortBy { get; set; } = "popularity.desc";
    public List<Genre> Genres { get; set; } = new();
    public List<Movie> Movies { get; set; } = new();
    public int TotalResults { get; set; }
    public int TotalPages { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
}

public class AccountProfileViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<int> Watchlist { get; set; } = new();
    public List<int> Favorites { get; set; } = new();
    public Dictionary<string, List<int>> CustomLists { get; set; } = new();
    public Dictionary<int, Movie> MoviesData { get; set; } = new();
    public int WatchlistCount { get; set; }
    public int FavoritesCount { get; set; }
    public int CustomListsCount { get; set; }
    public ChangePasswordViewModel ChangePassword { get; set; } = new();
}
