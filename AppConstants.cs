namespace MovieMood;

public static class AppConstants
{
    public static readonly Dictionary<string, string> HomeCategoryTitles = new()
    {
        ["trending"] = "Trending This Week",
        ["top_rated"] = "Top Rated of All Time",
        ["popular"] = "Popular",
        ["upcoming"] = "Coming Soon",
        ["now_playing"] = "Now Playing"
    };

    public static readonly Dictionary<string, MoodConfig> Moods = new()
    {
        ["happy"] = new("Happy", "fa-face-smile-beam", new[] { "Comedy", "Family" }, "#FFD700"),
        ["sad"] = new("Sad", "fa-cloud-rain", new[] { "Drama", "Romance" }, "#4682B4"),
        ["adventurous"] = new("Adventurous", "fa-compass", new[] { "Adventure", "Fantasy", "Action" }, "#32CD32"),
        ["scared"] = new("Scared", "fa-ghost", new[] { "Horror", "Thriller" }, "#8B0000"),
        ["romantic"] = new("Romantic", "fa-heart", new[] { "Romance", "Comedy" }, "#FF69B4"),
        ["angry"] = new("Angry", "fa-fire-flame-curved", new[] { "Action", "Crime" }, "#FF4500"),
        ["relaxed"] = new("Relaxed", "fa-mug-hot", new[] { "Animation", "Fantasy", "Drama" }, "#20B2AA")
    };
}

public record MoodConfig(string Label, string Icon, string[] GenreNames, string Color);
