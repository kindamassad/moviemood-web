using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMood.Data;
using MovieMood.Filters;
using MovieMood.Models;
using MovieMood.Services;
using MovieMood.ViewModels;

namespace MovieMood.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly MovieService _movieService;

    public AccountController(ApplicationDbContext db, MovieService movieService)
    {
        _db = db;
        _movieService = movieService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.IsLoggedIn())
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == model.Username.Trim());
        if (user == null || user.Password != model.Password)
        {
            model.ErrorMessage = user == null ? "User not found." : "Incorrect password.";
            return View(model);
        }

        HttpContext.Session.SignInUser(user.Id, user.Username);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.IsLoggedIn())
            return RedirectToAction("Index", "Home");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var username = model.Username.Trim();
        var email = model.Email.Trim();

        if (await _db.Users.AnyAsync(u => u.Username == username))
        {
            ModelState.AddModelError(nameof(model.Username), "Username already exists.");
            return View(model);
        }

        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "Email already exists.");
            return View(model);
        }

        var user = new User
        {
            Username = username,
            Email = email,
            Password = model.Password,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.SignOutUser();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [RequireLogin]
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetRequiredUserId();
        var user = await _db.Users.AsNoTracking().FirstAsync(u => u.Id == userId);
        var model = await BuildProfileViewModel(userId, user, new ChangePasswordViewModel());
        return View(model);
    }

    [HttpPost]
    [RequireLogin]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AccountProfileViewModel model)
    {
        var userId = HttpContext.Session.GetRequiredUserId();
        var user = await _db.Users.FirstAsync(u => u.Id == userId);
        var change = model.ChangePassword;

        if (string.IsNullOrWhiteSpace(change.CurrentPassword) ||
            string.IsNullOrWhiteSpace(change.NewPassword) ||
            string.IsNullOrWhiteSpace(change.ConfirmPassword))
        {
            change.ErrorMessage = "Please fill in all fields.";
        }
        else if (change.NewPassword.Length < 6)
        {
            change.ErrorMessage = "The new password must be at least 6 characters.";
        }
        else if (change.NewPassword != change.ConfirmPassword)
        {
            change.ErrorMessage = "Password confirmation does not match.";
        }
        else if (change.CurrentPassword != user.Password)
        {
            change.ErrorMessage = "Current password is incorrect.";
        }
        else
        {
            user.Password = change.NewPassword;
            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            change.SuccessMessage = "Password changed successfully.";
            change.CurrentPassword = string.Empty;
            change.NewPassword = string.Empty;
            change.ConfirmPassword = string.Empty;
        }

        return View(await BuildProfileViewModel(userId, user, change));
    }

    private async Task<AccountProfileViewModel> BuildProfileViewModel(int userId, User user, ChangePasswordViewModel changePassword)
    {
        var watchlist = await _db.UserLists
            .Where(ul => ul.UserId == userId && ul.ListType == "watchlist")
            .OrderByDescending(ul => ul.AddedAt)
            .Select(ul => ul.MovieId)
            .Take(5)
            .ToListAsync();

        var favorites = await _db.UserLists
            .Where(ul => ul.UserId == userId && ul.ListType == "favorites")
            .OrderByDescending(ul => ul.AddedAt)
            .Select(ul => ul.MovieId)
            .Take(5)
            .ToListAsync();

        var customListNames = await _db.UserLists
            .Where(ul => ul.UserId == userId && ul.ListType == "custom")
            .Select(ul => ul.ListName!)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync();

        var customLists = new Dictionary<string, List<int>>();
        foreach (var listName in customListNames)
        {
            var ids = await _db.UserLists
                .Where(ul => ul.UserId == userId && ul.ListType == "custom" && ul.ListName == listName)
                .OrderByDescending(ul => ul.AddedAt)
                .Select(ul => ul.MovieId)
                .Take(5)
                .ToListAsync();
            customLists[listName] = ids;
        }

        var allMovieIds = watchlist
            .Concat(favorites)
            .Concat(customLists.Values.SelectMany(x => x))
            .Distinct()
            .ToList();

        var moviesData = await _movieService.GetMoviesByIdsAsync(allMovieIds);

        return new AccountProfileViewModel
        {
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Watchlist = watchlist,
            Favorites = favorites,
            CustomLists = customLists,
            MoviesData = moviesData,
            WatchlistCount = await _db.UserLists.CountAsync(ul => ul.UserId == userId && ul.ListType == "watchlist"),
            FavoritesCount = await _db.UserLists.CountAsync(ul => ul.UserId == userId && ul.ListType == "favorites"),
            CustomListsCount = customListNames.Count,
            ChangePassword = changePassword
        };
    }
}
