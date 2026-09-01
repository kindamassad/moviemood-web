IF DB_ID(N'moviemood_database') IS NULL
    CREATE DATABASE moviemood_database;
GO

USE moviemood_database;
GO

IF OBJECT_ID(N'dbo.user_lists', N'U') IS NOT NULL DROP TABLE user_lists;
IF OBJECT_ID(N'dbo.users', N'U') IS NOT NULL DROP TABLE users;
IF OBJECT_ID(N'dbo.cast_members', N'U') IS NOT NULL DROP TABLE cast_members;
IF OBJECT_ID(N'dbo.category_movies', N'U') IS NOT NULL DROP TABLE category_movies;
IF OBJECT_ID(N'dbo.categories', N'U') IS NOT NULL DROP TABLE categories;
IF OBJECT_ID(N'dbo.movie_genres', N'U') IS NOT NULL DROP TABLE movie_genres;
IF OBJECT_ID(N'dbo.movies', N'U') IS NOT NULL DROP TABLE movies;
IF OBJECT_ID(N'dbo.genres', N'U') IS NOT NULL DROP TABLE genres;
GO

CREATE TABLE genres (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) NOT NULL UNIQUE
);

SET IDENTITY_INSERT genres ON;
INSERT INTO genres (id, name) VALUES
(1,  N'Action'),
(2,  N'Adventure'),
(3,  N'Animation'),
(4,  N'Comedy'),
(5,  N'Crime'),
(6,  N'Drama'),
(7,  N'Family'),
(8,  N'Fantasy'),
(9,  N'Horror'),
(10, N'Romance'),
(11, N'Sci-Fi'),
(12, N'Thriller');
SET IDENTITY_INSERT genres OFF;

CREATE TABLE movies (
    id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(200) NOT NULL,
    tagline NVARCHAR(255) NULL,
    overview NVARCHAR(MAX) NOT NULL,
    poster_path NVARCHAR(255) NULL,
    backdrop_path NVARCHAR(255) NULL,
    release_date DATE NULL,
    vote_average FLOAT NOT NULL DEFAULT 0,
    runtime INT NULL,
    original_language NVARCHAR(10) NOT NULL DEFAULT N'en',
    director NVARCHAR(100) NULL,
    trailer_key NVARCHAR(50) NULL,
    genre_id INT NOT NULL,
    CONSTRAINT FK_movies_genres FOREIGN KEY (genre_id) REFERENCES genres(id)
);

SET IDENTITY_INSERT movies ON;
INSERT INTO movies (id, title, tagline, overview, poster_path, backdrop_path, release_date, vote_average, runtime, original_language, director, trailer_key, genre_id) VALUES
(1,  N'The Shawshank Redemption', N'Fear can hold you prisoner. Hope can set you free.',
     N'Two imprisoned men bond over years, finding solace and eventual redemption through acts of common decency.',
     N'/images/posters/01-shawshank.jpg', N'/images/posters/01-shawshank.jpg', '1994-09-23', 9.3, 142, N'en', N'Frank Darabont', N'6hB3IaOhO6Q', 6),
(2,  N'The Dark Knight', N'Why So Serious?',
     N'Batman must accept one of the greatest psychological tests to fight the injustice brought by the Joker.',
     N'/images/posters/02-dark-knight.jpg', N'/images/posters/02-dark-knight.jpg', '2008-07-18', 9.0, 152, N'en', N'Christopher Nolan', N'EXeTwQWrcwY', 1),
(3,  N'Inception', N'Your mind is the scene of the crime.',
     N'A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea.',
     N'/images/posters/03-inception.jpg', N'/images/posters/03-inception.jpg', '2010-07-16', 8.8, 148, N'en', N'Christopher Nolan', N'YoHD9XEInc0', 11),
(4,  N'Forrest Gump', N'Life is like a box of chocolates.',
     N'The presidencies of Kennedy and Johnson, the Vietnam War, and other history unfold through the perspective of an Alabama man.',
     N'/images/posters/04-forrest-gump.jpg', N'/images/posters/04-forrest-gump.jpg', '1994-07-06', 8.8, 142, N'en', N'Robert Zemeckis', N'bLvqoHBptjk', 6),
(5,  N'The Lion King', N'The Circle of Life.',
     N'Lion prince Simba flees his kingdom after the murder of his father, only to learn the true meaning of responsibility.',
     N'/images/posters/05-lion-king.jpg', N'/images/posters/05-lion-king.jpg', '1994-06-24', 8.5, 88,  N'en', N'Roger Allers', N'4sj1MT05lAA', 3),
(6,  N'Toy Story', N'The adventure takes off when toys come to life.',
     N'A cowboy doll is profoundly threatened when a new spaceman action figure supplants him as top toy in a boy''s room.',
     N'/images/posters/06-toy-story.jpg', N'/images/posters/06-toy-story.jpg', '1995-11-22', 8.3, 81,  N'en', N'John Lasseter', N'KYz2wyWjjEw', 3),
(7,  N'The Conjuring', N'Based on the true case files of the Warrens.',
     N'Paranormal investigators Ed and Lorraine Warren work to help a family terrorized by a dark presence in their farmhouse.',
     N'/images/posters/07-conjuring.jpg', N'/images/posters/07-conjuring.jpg', '2013-07-19', 7.5, 112, N'en', N'James Wan', N'ejMMo0R7FpA', 9),
(8,  N'Get Out', N'Just because you are invited, does not mean you are welcome.',
     N'A young African-American visits his white girlfriend''s parents for the weekend, where his uneasiness about their reception turns into terror.',
     N'/images/posters/08-get-out.png', N'/images/posters/08-get-out.png', '2017-02-24', 7.7, 104, N'en', N'Jordan Peele', N'Dzfpy-XlxqM', 9),
(9,  N'La La Land', N'Here''s to the fools who dream.',
     N'While navigating their careers in Los Angeles, a pianist and an actress fall in love while attempting to reconcile aspirations and relationship.',
     N'/images/posters/09-la-la-land.png', N'/images/posters/09-la-la-land.png', '2016-12-09', 8.0, 128, N'en', N'Damien Chazelle', N'0pdqf4P9M8A', 10),
(10, N'When Harry Met Sally', N'Can two friends sleep together and still love each other in the morning?',
     N'Harry and Sally have known each other for years, and are very good friends, but they fear sex would ruin the friendship.',
     N'/images/posters/10-harry-sally.jpg', N'/images/posters/10-harry-sally.jpg', '1989-07-21', 7.6, 96,  N'en', N'Rob Reiner', N'1A2WS9Yf7pk', 10),
(11, N'Mad Max: Fury Road', N'What a lovely day.',
     N'In a post-apocalyptic wasteland, Max teams up with a mysterious woman to escape from a tyrannical warlord.',
     N'/images/posters/11-mad-max.jpg', N'/images/posters/11-mad-max.jpg', '2015-05-15', 8.1, 120, N'en', N'George Miller', N'hEJnniGwdlA', 1),
(12, N'The Lord of the Rings: The Fellowship of the Ring', N'One ring to rule them all.',
     N'A meek Hobbit and eight companions set out on a journey to destroy the powerful One Ring and save Middle-earth.',
     N'/images/posters/12-lotr.jpg', N'/images/posters/12-lotr.jpg', '2001-12-19', 8.8, 178, N'en', N'Peter Jackson', N'V75dMMIW2B4', 8),
(13, N'Finding Nemo', N'There are 3.7 trillion fish in the ocean. They''re looking for one.',
     N'After his son is captured in the Great Barrier Reef, a clownfish sets out on a journey to bring him home.',
     N'/images/posters/13-finding-nemo.jpg', N'/images/posters/13-finding-nemo.jpg', '2003-05-30', 8.1, 100, N'en', N'Andrew Stanton', N'wZdpNglLkb8', 3),
(14, N'The Godfather', N'An offer you can''t refuse.',
     N'The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.',
     N'/images/posters/14-godfather.jpg', N'/images/posters/14-godfather.jpg', '1972-03-24', 9.2, 175, N'en', N'Francis Ford Coppola', N'sY1S34973zA', 5),
(15, N'Parasite', N'Act like you own the place.',
     N'Greed and class discrimination threaten the newly formed symbiotic relationship between the wealthy Park family and the destitute Kim clan.',
     N'/images/posters/15-parasite.png', N'/images/posters/15-parasite.png', '2019-11-08', 8.5, 132, N'ko', N'Bong Joon-ho', N'5xH0HfJHsaY', 12),
(16, N'Spirited Away', N'On the other side of the tunnel was a mysterious town.',
     N'During her family''s move to the suburbs, a sullen 10-year-old wanders into a world ruled by gods, witches, and spirits.',
     N'/images/posters/16-spirited-away.png', N'/images/posters/16-spirited-away.png', '2001-07-20', 8.6, 125, N'ja', N'Hayao Miyazaki', N'ByXuk9QqQkk', 3),
(17, N'Dune', N'It begins.',
     N'Feature adaptation of Frank Herbert''s science fiction novel about the son of a noble family entrusted with protecting the most valuable asset in the galaxy.',
     N'/images/posters/17-dune.jpg', N'/images/posters/17-dune.jpg', '2021-10-22', 8.0, 155, N'en', N'Denis Villeneuve', N'n9xhJrPXop4', 11),
(18, N'Everything Everywhere All at Once', N'The universe is so much bigger than you realize.',
     N'An aging Chinese immigrant is swept up in an insane adventure in which she alone can save the world by exploring other universes.',
     N'/images/posters/18-everything.jpg', N'/images/posters/18-everything.jpg', '2022-03-25', 8.1, 139, N'en', N'Daniel Kwan', N'wxN1T1uxQ2g', 1);
SET IDENTITY_INSERT movies OFF;

CREATE TABLE movie_genres (
    movie_id INT NOT NULL,
    genre_id INT NOT NULL,
    PRIMARY KEY (movie_id, genre_id),
    CONSTRAINT FK_movie_genres_movies FOREIGN KEY (movie_id) REFERENCES movies(id) ON DELETE CASCADE,
    CONSTRAINT FK_movie_genres_genres FOREIGN KEY (genre_id) REFERENCES genres(id) ON DELETE CASCADE
);

INSERT INTO movie_genres (movie_id, genre_id) VALUES
(2,  5),  (2,  12),
(3,  1),  (3,  12),
(4,  10),
(5,  7),  (5,  8),
(6,  4),  (6,  7),
(7,  12),
(8,  12),
(9,  6),
(10, 4),
(11, 2),
(12, 2),
(14, 6),
(15, 6), (15, 5),
(16, 8),
(17, 2),
(18, 4);

CREATE TABLE categories (
    id INT IDENTITY(1,1) PRIMARY KEY,
    slug NVARCHAR(50) NOT NULL UNIQUE,
    title NVARCHAR(100) NOT NULL
);

SET IDENTITY_INSERT categories ON;
INSERT INTO categories (id, slug, title) VALUES
(1, N'trending',    N'Trending This Week'),
(2, N'top_rated',   N'Top Rated of All Time'),
(3, N'popular',     N'Popular'),
(4, N'upcoming',    N'Coming Soon'),
(5, N'now_playing', N'Now Playing');
SET IDENTITY_INSERT categories OFF;

CREATE TABLE category_movies (
    category_id INT NOT NULL,
    movie_id INT NOT NULL,
    display_order INT NOT NULL DEFAULT 0,
    PRIMARY KEY (category_id, movie_id),
    CONSTRAINT FK_category_movies_categories FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE,
    CONSTRAINT FK_category_movies_movies FOREIGN KEY (movie_id) REFERENCES movies(id) ON DELETE CASCADE
);

INSERT INTO category_movies (category_id, movie_id, display_order) VALUES
(1, 18, 1), (1, 17, 2), (1, 15, 3), (1, 11, 4), (1, 2,  5), (1, 3,  6),
(2, 1,  1), (2, 14, 2), (2, 2,  3), (2, 12, 4), (2, 3,  5), (2, 16, 6),
(3, 5,  1), (3, 6,  2), (3, 13, 3), (3, 4,  4), (3, 9,  5), (3, 10, 6),
(4, 17, 1), (4, 18, 2), (4, 15, 3), (4, 8,  4), (4, 7,  5), (4, 11, 6),
(5, 2,  1), (5, 11, 2), (5, 7,  3), (5, 8,  4), (5, 9,  5), (5, 3,  6);

CREATE TABLE cast_members (
    id INT IDENTITY(1,1) PRIMARY KEY,
    movie_id INT NOT NULL,
    name NVARCHAR(100) NOT NULL,
    character_name NVARCHAR(100) NOT NULL,
    photo_path NVARCHAR(255) NULL,
    CONSTRAINT FK_cast_members_movies FOREIGN KEY (movie_id) REFERENCES movies(id) ON DELETE CASCADE
);

INSERT INTO cast_members (movie_id, name, character_name, photo_path) VALUES
(1,  N'Tim Robbins',       N'Andy Dufresne',        N'/images/cast/m1-c1.svg'),
(1,  N'Morgan Freeman',    N'Ellis Boyd Redding',   N'/images/cast/m1-c2.svg'),
(2,  N'Christian Bale',    N'Bruce Wayne',          N'/images/cast/m2-c1.svg'),
(2,  N'Heath Ledger',      N'Joker',                N'/images/cast/m2-c2.svg'),
(3,  N'Leonardo DiCaprio', N'Cobb',                 N'/images/cast/m3-c1.svg'),
(3,  N'Joseph Gordon-Levitt', N'Arthur',            N'/images/cast/m3-c2.svg'),
(4,  N'Tom Hanks',         N'Forrest Gump',         N'/images/cast/m4-c1.svg'),
(4,  N'Robin Wright',      N'Jenny Curran',         N'/images/cast/m4-c2.svg'),
(5,  N'Matthew Broderick', N'Adult Simba',          N'/images/cast/m5-c1.svg'),
(5,  N'James Earl Jones',  N'Mufasa',               N'/images/cast/m5-c2.svg'),
(6,  N'Tom Hanks',         N'Woody',                N'/images/cast/m6-c1.svg'),
(6,  N'Tim Allen',         N'Buzz Lightyear',       N'/images/cast/m6-c2.svg'),
(7,  N'Vera Farmiga',      N'Lorraine Warren',      N'/images/cast/m7-c1.svg'),
(7,  N'Patrick Wilson',    N'Ed Warren',            N'/images/cast/m7-c2.svg'),
(8,  N'Daniel Kaluuya',    N'Chris Washington',     N'/images/cast/m8-c1.svg'),
(8,  N'Allison Williams',  N'Rose Armitage',        N'/images/cast/m8-c2.svg'),
(9,  N'Ryan Gosling',      N'Sebastian',            N'/images/cast/m9-c1.svg'),
(9,  N'Emma Stone',        N'Mia',                  N'/images/cast/m9-c2.svg'),
(10, N'Billy Crystal',     N'Harry Burns',          N'/images/cast/m10-c1.svg'),
(10, N'Meg Ryan',          N'Sally Albright',       N'/images/cast/m10-c2.svg'),
(11, N'Tom Hardy',         N'Max Rockatansky',      N'/images/cast/m11-c1.svg'),
(11, N'Charlize Theron',   N'Imperator Furiosa',    N'/images/cast/m11-c2.svg'),
(12, N'Elijah Wood',       N'Frodo Baggins',        N'/images/cast/m12-c1.svg'),
(12, N'Ian McKellen',      N'Gandalf',              N'/images/cast/m12-c2.svg'),
(13, N'Albert Brooks',     N'Marlin',               N'/images/cast/m13-c1.svg'),
(13, N'Ellen DeGeneres',   N'Dory',                 N'/images/cast/m13-c2.svg'),
(14, N'Marlon Brando',     N'Don Vito Corleone',    N'/images/cast/m14-c1.svg'),
(14, N'Al Pacino',         N'Michael Corleone',     N'/images/cast/m14-c2.svg'),
(15, N'Song Kang-ho',      N'Kim Ki-taek',          N'/images/cast/m15-c1.svg'),
(15, N'Choi Woo-shik',     N'Kim Ki-woo',           N'/images/cast/m15-c2.svg'),
(16, N'Rumi Hiiragi',      N'Chihiro',              N'/images/cast/m16-c1.svg'),
(16, N'Miyu Irino',        N'Haku',                 N'/images/cast/m16-c2.svg'),
(17, N'Timothée Chalamet', N'Paul Atreides',        N'/images/cast/m17-c1.svg'),
(17, N'Zendaya',           N'Chani',                N'/images/cast/m17-c2.svg'),
(18, N'Michelle Yeoh',     N'Evelyn Wang',          N'/images/cast/m18-c1.svg'),
(18, N'Ke Huy Quan',       N'Waymond Wang',         N'/images/cast/m18-c2.svg');

CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50) NOT NULL UNIQUE,
    email NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE user_lists (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    movie_id INT NOT NULL,
    list_type NVARCHAR(20) NOT NULL,
    list_name NVARCHAR(50) NULL,
    added_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_user_lists_users FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT FK_user_lists_movies FOREIGN KEY (movie_id) REFERENCES movies(id) ON DELETE CASCADE,
    CONSTRAINT CK_user_lists_list_type CHECK (list_type IN (N'watchlist', N'favorites', N'custom')),
    CONSTRAINT unique_user_movie_list UNIQUE (user_id, movie_id, list_type, list_name)
);
GO
