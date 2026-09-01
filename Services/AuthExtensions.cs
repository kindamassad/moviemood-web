using Microsoft.AspNetCore.Http;

namespace MovieMood.Services;

public static class SessionAuthExtensions
{
    public const string UserIdKey = "UserId";
    public const string UsernameKey = "Username";

    public static bool IsLoggedIn(this ISession session)
        => session.GetInt32(UserIdKey).HasValue;

    public static int? GetUserId(this ISession session)
        => session.GetInt32(UserIdKey);

    public static string? GetUsername(this ISession session)
        => session.GetString(UsernameKey);

    public static int GetRequiredUserId(this ISession session)
    {
        var userId = session.GetInt32(UserIdKey);
        if (!userId.HasValue)
            throw new InvalidOperationException("User is not logged in.");

        return userId.Value;
    }

    public static void SignInUser(this ISession session, int userId, string username)
    {
        session.SetInt32(UserIdKey, userId);
        session.SetString(UsernameKey, username);
    }

    public static void SignOutUser(this ISession session)
        => session.Clear();
}
