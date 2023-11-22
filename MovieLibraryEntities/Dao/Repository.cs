using Microsoft.EntityFrameworkCore;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MovieLibraryEntities.Dao
{
    public class Repository : IRepository, IDisposable
    {
        private readonly IDbContextFactory<MovieContext> _contextFactory;
        private readonly MovieContext _context;

        public Repository(IDbContextFactory<MovieContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _context = _contextFactory.CreateDbContext();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IEnumerable<Movie> GetAll()
        {
            var allMovies = _context.Movies
                .Include(x => x.MovieGenres)
                .ThenInclude(x => x.Genre)
                .ToList();

            return allMovies;

        }

        public IEnumerable<Movie> GetTopMovies(int amount)
        {
            var allMovies = _context.Movies
                .Include(x => x.MovieGenres)
                .ThenInclude(x => x.Genre)
                .OrderBy(x => x.Title)
                .Take(amount);

            return allMovies;
        }

        public IEnumerable<Movie> Search(string searchString)
        {
            var yearPattern = @"\(\d\d\d\d\)"; //year pattern to match any year between 1800 and 2099.

            var allMovies = _context.Movies
                .Include(x => x.MovieGenres)
                .ThenInclude(x => x.Genre);
            var listOfMovies = allMovies.ToList();

            var temp = listOfMovies.Where(x => Regex.Replace(x.Title, yearPattern, "").Contains(searchString, StringComparison.CurrentCultureIgnoreCase));
            return temp;
        }

        public IEnumerable<Movie> SearchByReleaseYear(int releaseYear)
        {
            var allMovies = _context.Movies
            .Include(x => x.MovieGenres)
            .ThenInclude(x => x.Genre);
            var listOfMovies = allMovies.ToList();

            var temp = listOfMovies.Where(x => x.ReleaseDate.Year == releaseYear);
            return temp;
        }

        public void AddMovie(string movieTitle, DateTime releaseDate, string releaseYear, List<int>? movieGenres)
        {
            var movie = new Movie()
            {
                Title = movieTitle + " (" + releaseYear + ")",
                ReleaseDate = releaseDate
            };
            //check if movie exists. If it does, exit method.
            var allMovies = _context.Movies;
            var listOfMovies = allMovies.ToList();
            if(listOfMovies.Exists(x => x.Title == movie.Title))
            {
                Console.WriteLine($"\nError: {movie.Title} already exists in the database. Entry not added.\n");
                return;
            }
            //add movie genre(s)

            //if there are no genres to add, save add movie to database.
            if (movieGenres is null || !movieGenres.Any())
            {
                _context.Movies.Add(movie);
                _context.SaveChanges();
                return;
            }

            var userGenres = new List<MovieGenre>();
            foreach (var genre in movieGenres)
            {
                var movieGenre = new MovieGenre()
                {
                    Genre = _context.Genres.FirstOrDefault(x => x.Id == genre),
                    Movie = movie
                };
                userGenres.Add(movieGenre);
            }
            _context.Movies.Add(movie);

            foreach (var userGenre in userGenres)
            {
                _context.MovieGenres.Add(userGenre);
            }
            _context.SaveChanges();

            //send message to user indicating the movie has been added.
            Console.WriteLine($"{movie.Title} added to database.\n");
        }

        public void DisplayMovieDetails(int movieID)
        {
            var movie = SearchByID(movieID);

            Console.WriteLine($"\nMovie ID: {movie.Id}");
            Console.WriteLine($"Movie Title (Release Year): {movie.Title}");
            Console.WriteLine($"Release Date: {movie.ReleaseDate.ToString("MMMM dd, yyyy")}");
            if (movie.MovieGenres is not null && movie.MovieGenres.Count() > 0)
            {
                Console.Write("Movie Genre(s): ");
                for (int i = 0; i < movie.MovieGenres.Count(); i++)
                {
                    if (i != (movie.MovieGenres.Count() - 1))
                    {
                        Console.Write($"{movie.MovieGenres.ElementAt(i).Genre.Name}, ");
                    }
                    else
                        Console.WriteLine($"{movie.MovieGenres.ElementAt(i).Genre.Name}\n");
                }
            }
        }

        public Movie SearchByID (int movieID)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == movieID);

            return movie;
        }

        public bool GetValidMovie(int movieID)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == movieID);
            if (movie is Movie)
                return true;
            else
                //movie is default(Movie); FirstOrDefault returns default(Type)
                return false;
        }

        //Get a listing of avaialble movie genres in the Genres table.
        public IEnumerable<Genre> GetGenres()
        {
            return _context.Genres;
        }

        //update movie title
        public bool UpdateMovieTitle(int movieID, string movieTitle)
        {
            var movie = SearchByID(movieID);
            //recreate the movie title using the movie's current year to
            //match formatting.
            movie.Title = $"{movieTitle} ({movie.ReleaseDate.Year})";
            _context.SaveChanges();
            return true;
        }

        //update movie release year
        public bool UpdateMovieReleaseYear(int movieID, int releaseYear)
        {
            var movie = SearchByID(movieID);
            //recreate the movie title using the movie's current title to
            //match formating.
            var yearIndex = Regex.Match(movie.Title, @"\(\d\d\d\d\)").Index;
            var titleOnly = movie.Title.Substring(0, yearIndex);
            movie.Title = $"{titleOnly.Trim()} ({releaseYear})";

            //update release date with a new year. There has to be a simpler way
            //to accomplish this ...
            movie.ReleaseDate = Convert.ToDateTime($"{Convert.ToString(releaseYear)}-{Convert.ToString(movie.ReleaseDate.Month)}-{Convert.ToString(movie.ReleaseDate.Day)}");
            _context.SaveChanges();
            return true;
        }

        //update movie release date
        public bool UpdateMovieReleaseDate(int movieID, DateTime releaseDate)
        {
            var movie = SearchByID(movieID);

            //recreate the movie title (year) using the movie's current title to
            //match formating.
            var yearIndex = Regex.Match(movie.Title, @"\(\d\d\d\d\)").Index;
            var titleOnly = movie.Title.Substring(0, yearIndex);
            movie.Title = $"{titleOnly.Trim()} ({releaseDate.Year})";

            movie.ReleaseDate = releaseDate;
            _context.SaveChanges();
            

            return true;
        }

        //add movie genre(s)
        public bool AddMovieGenres(int movieID, List<int> movieGenres)
        {
            var movie = SearchByID(movieID);
            var userGenres = new List<MovieGenre>();

            foreach (var genre in movieGenres)
            {
                var movieGenre = new MovieGenre()
                {
                    Genre = _context.Genres.FirstOrDefault(x => x.Id == genre),
                    Movie = movie
                };
                userGenres.Add(movieGenre);
            }
            foreach (var userGenre in userGenres)
            {
                _context.MovieGenres.Add(userGenre);
            }
            _context.SaveChanges();
            return true;
        }

        //update movie genres
        public bool UpdateMovieGenres(int movieID, List<int> movieGenres)
        {
            var movie = SearchByID(movieID);
            var userGenres = new List<MovieGenre>();

            foreach (var genre in movieGenres)
            {
                var movieGenre = new MovieGenre()
                {
                    Genre = _context.Genres.FirstOrDefault(x => x.Id == genre),
                    Movie = movie
                };
                userGenres.Add(movieGenre);
            }
            foreach (var userGenre in userGenres)
            {
                movie.MovieGenres = userGenres;
            }
            _context.SaveChanges();
            return true;
        }

        public bool DeleteMovieGenre (int movieID, int deleteGenre)
        {
            var movie = SearchByID(movieID);
            var currentGenres = movie.MovieGenres ?? new List<MovieGenre>();

            currentGenres.Remove(movie.MovieGenres.FirstOrDefault(x => x.Genre.Id ==
                currentGenres.ElementAt(deleteGenre - 1).Genre.Id));

            _context.SaveChanges();

            return true;
        }

        public void DeleteMovie(int movieID)
        {
            var movieToDelete = SearchByID(movieID);
            _context.Remove(movieToDelete);
            _context.SaveChanges();
            return;
        }
    }
}