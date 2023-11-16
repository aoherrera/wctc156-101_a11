using Microsoft.EntityFrameworkCore;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;

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
            var allMovies = _context.Movies.ToList();
            return allMovies;

        }

        public IEnumerable<Movie> Search(string searchString)
        {
            var allMovies = _context.Movies;
            var listOfMovies = allMovies.ToList();
            var temp = listOfMovies.Where(x => x.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));
            return temp;
        }

        public void AddMovie(string movieTitle, DateTime releaseDate, string releaseYear)
        {


            var movie = new Movie()
            {
                Title = movieTitle + " (" + releaseYear + ")",
                ReleaseDate = releaseDate
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();

            Console.WriteLine($"{movie.Title} added to database.");
        }
    }
}