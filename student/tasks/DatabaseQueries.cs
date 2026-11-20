using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using tasks.Databases;

namespace tasks;

/// <summary>
/// Here you should implement various database query methods for the IMovieDatabase interface.
/// </summary>
public static class DatabaseQueries
{
    public static void RunQueries(this IMovieDatabase movieDatabase)
    {
        movieDatabase.ActorsFromFantasyMovies();
        movieDatabase.LongestMovieByGenre();
        movieDatabase.HighRatedMoviesWithCast();
        movieDatabase.DistinctRolesCountPerActor();
        movieDatabase.RecentMoviesWithAverageRating();
        movieDatabase.AverageRatingByGenre();
        movieDatabase.ActorsWhoNeverPlayedInThriller();
        movieDatabase.Top3MoviesByRatingCount();
        movieDatabase.MoviesWithoutRatings();
        movieDatabase.MostVersatileActors();
    }

    public static void ActorsFromFantasyMovies(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = actors
        .Join(
            casts,
            leftItem => leftItem.Id,
            rightItem => rightItem.ActorId,
            (left, right) =>
            {
                return new
                {
                    Name = left.Name,
                    MovieId = right.MovieId,
                };
            }
        ).Join(
            movies,
            leftItem => leftItem.MovieId,
            rightItem => rightItem.Id,
            (left, right) =>
            {
                return new
                {
                  left.Name,
                  right.Genre  
                };
            }
        )
        .Where(item => item.Genre == Genre.Fantasy)
        .Select(item => item.Name)
        .Distinct()
        .ToList();




        Console.WriteLine("Actors From Fantasy Movies");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void LongestMovieByGenre(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = movies
        .GroupBy(item=>item.Genre)
        .Select(group => group.OrderByDescending(movie=>movie.DurationMinutes).First())
        .ToList();   

        Console.WriteLine("Longest Movie By Genre");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void HighRatedMoviesWithCast(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = ratings
        .GroupBy(obj => obj.MovieId)
        .Select(obj =>
        {
            return new
            {
                Key =  obj.Key,
                Average = obj.Average((movie)=>movie.Score),
            };
        })
        .Join(
            movies,
            left => left.Key,
            right => right.Id,
            (left, right) =>
            {
                return new {
                    right.Id,
                    left.Average,
                    right.Title};
            } 
        )
        .Where(id => id.Average > 8)
        .Join(
            casts,
            left => left.Id,
            right => right.MovieId,
            (left,right)=>
            {
                return new
                {
                    left.Id,
                    left.Title,
                    left.Average,
                    right.ActorId
                };  
            }
        )
        .Join(
            actors,
            left => left.ActorId,
            right => right.Id,
            (movie, right) =>
            {
                return new
                {
                    movie.Id,
                    movie.Title,
                    movie.Average,
                    right.Name  
                };
            }
        )
        .GroupBy(obj=>obj.Id)
        .Select(group  => new {group.First().Title , group.First().Average, Actors = group.Select(item => item.Name) })
        .ToList();

        Console.WriteLine("High Rated Movies With Cast");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void DistinctRolesCountPerActor(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = actors
        .Join(
            casts,
            left => left.Id,
            right => right.ActorId,
            (left, right) =>
            {
                return new
                {
                  left.Name,
                  right.MovieId  
                };
            }
        )
        .GroupBy(cast => cast.Name)
        .Select(obj => new {obj.Key , count = obj.Count()})
        .OrderByDescending(obj => obj.count);

        Console.WriteLine("Distinct Roles Count Per Actor");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void RecentMoviesWithAverageRating(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = new object();

        Console.WriteLine("Recent Movies With Average Rating");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void AverageRatingByGenre(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = ratings.GroupBy(obj => obj.MovieId)
        .Select(obj =>
        {
            return new
            {   
                Key =  obj.Key,
                Average = obj.Average((movie)=>movie.Score),
            };
        })
        .Join(
            movies,
            left => left.Key,
            right => right.Id,
            (left, right) =>
            {
                return new {
                    right.Year,
                    right.Id,
                    left.Average,
                    right.Title};
            } 
        )
        .OrderByDescending( ob => ob.Average)
        .Where(obj =>  2026 - obj.Year < 5)
        .Select(obj => new {obj.Title, obj.Average})
        .ToList();
        
        Console.WriteLine("Average Rating By Genre");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void ActorsWhoNeverPlayedInThriller(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = new object();

        Console.WriteLine("Actors Who Never Played In Thriller");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void Top3MoviesByRatingCount(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = movies.GroupJoin(
            ratings,
            left => left.Id,
            right=>right.MovieId,
            (left, right) =>
            {
                return new {
                    left.Title,
                    count = right.Count()
                    };
            }
            
        )
        .OrderByDescending(obj => obj.count)
        .Take(3)
        .ToList();

        Console.WriteLine("Top 3 Movies By Rating Count");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void MoviesWithoutRatings(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = movies.GroupJoin(
            ratings,
            movie => movie.Id,
            rating => rating.MovieId,
            (movie, group) =>
            {
                return new
                {
                  name = movie.Title,
                  count = group.Count()  
                };
            }
        ).Where(obj=> obj.count == 0)
        .Select(obj => obj.name)
        .ToList();

        Console.WriteLine("Movies Without Ratings");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void MostVersatileActors(this IMovieDatabase movieDatabase)
    {
        var movies = movieDatabase.Movies;
        var actors = movieDatabase.Actors;
        var ratings = movieDatabase.Ratings;
        var casts = movieDatabase.Casts;

        var queryResult = movies.Join(
            casts,
            movie=>movie.Id,
            cast => cast.MovieId,
            ( movie, cast ) =>
            new {
               movie.Genre,
               cast.ActorId,  
            }
        ).Join(
            actors,
            left => left.ActorId,
            right => right.Id,
            (left, right) =>
            new {
                right.Name,
                left.Genre
            }
        )
        .GroupBy(na => na.Name)
        .Select(obj => new {obj.Key, r = obj.Distinct().Count()})
        .OrderByDescending(obj => obj.r)
        .Take(3)
        .ToList();

        Console.WriteLine("Most Versatile Actors");
        DisplayQueryResults(queryResult);
        Console.WriteLine();
    }

    public static void DisplayQueryResults<T>(T query)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        options.Converters.Add(new JsonStringEnumConverter());

        var json = JsonSerializer.Serialize(query, options);

        Console.WriteLine(json);
    }
}