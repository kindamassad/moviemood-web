# MovieMood

ASP.NET Core MVC web application for exploring movies, managing personal lists, and mood-based recommendations. All movie data is stored in SQL Server LocalDB — no external APIs.

## Features

- **User Authentication**: Register, login, logout (Session state)
- **Movie Exploration**: Trending, Top Rated, Popular, Upcoming, Now Playing
- **Search & Discover**: Filters by genre, year, rating, and sort order
- **Movie Details**: Synopsis, cast, trailer, similar movies
- **Personal Lists**: Watchlist, favorites, custom lists
- **Mood Picker**: 3 movie suggestions based on your mood
- **Change Password**: From the profile page

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | C# / ASP.NET Core MVC (.NET 6) |
| Views | HTML + Razor (`@model`, `@foreach`, Tag Helpers) |
| Styling | CSS + Bootstrap 5 |
| Database | SQL Server LocalDB + EF Core (DbContext, LINQ) |
| Frontend JS | jQuery, Bootstrap (LibMan), custom JavaScript |

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [.NET 6 ASP.NET Core Runtime](https://dotnet.microsoft.com/download/dotnet/6.0) (required to run the app)
- Visual Studio 2022 with **ASP.NET and web development** workload (includes SQL Server LocalDB)

## Setup

1. **Clone the project** and open the folder in Visual Studio.

2. **Create the database** — import the schema and seed data:

   **Option A — SQL Server Object Explorer (Visual Studio):**
   - View → SQL Server Object Explorer
   - Connect to `(localdb)\MSSQLLocalDB`
   - Right-click the server → **New Query**
   - Open `database/moviemood.sql`, paste/run the script

   **Option B — Command line:**
   ```powershell
   sqlcmd -S "(localdb)\MSSQLLocalDB" -i database\moviemood.sql
   ```

3. **Configure connection** (optional):

   Copy `appsettings.Local.json.example` to `appsettings.Local.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=moviemood_database;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
     }
   }
   ```

4. **Run the app:**

   Press **F5** in Visual Studio, or from the terminal:

   ```bash
   dotnet run
   ```

   Open `http://localhost:5107` (see console output).

## Troubleshooting

**SqlException / LocalDB not found:**
1. Install Visual Studio workload **ASP.NET and web development**, or [SQL Server Express LocalDB](https://go.microsoft.com/fwlink/?linkid=866658)
2. Run `database/moviemood.sql` on `(localdb)\MSSQLLocalDB`
3. In Visual Studio: View → SQL Server Object Explorer → connect to `(localdb)\MSSQLLocalDB`

**".NET 6 not found" when running:**
Install [.NET 6 ASP.NET Core Runtime](https://dotnet.microsoft.com/download/dotnet/6.0) (not just the SDK).

## Project Structure

```
moviemood/
├── Controllers/          # MVC controllers (C#)
├── Data/                 # ApplicationDbContext (EF Core)
├── Models/               # User, Movie, Genre, CastMember, etc.
├── Services/             # MovieService, Auth
├── Views/                # Razor pages (.cshtml)
├── wwwroot/              # CSS, JS, images (local static files)
│   └── images/           # Posters, backdrops, cast photos
├── database/moviemood.sql  # SQL Server schema + seed data (for ERD)
├── Program.cs            # App startup & DI
└── appsettings.json      # Configuration
```

## Database Tables (ERD)

| Table | Description |
|-------|-------------|
| `genres` | Movie genres (Action, Drama, etc.) |
| `movies` | Movie catalog (title, overview, rating, director, etc.) |
| `movie_genres` | Many-to-many: movies ↔ genres |
| `categories` | Home page rows (trending, popular, etc.) |
| `category_movies` | Movies assigned to each category |
| `cast_members` | Cast per movie |
| `users` | User accounts |
| `user_lists` | Watchlist / favorites / custom lists |

Use `database/moviemood.sql` to draw your ERD diagram.
