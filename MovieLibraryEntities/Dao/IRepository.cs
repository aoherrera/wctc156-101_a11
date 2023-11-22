using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public interface IRepository
    {
        IEnumerable<Movie> GetAll();
        IEnumerable<Movie> GetTopMovies(int amount);
        IEnumerable<Movie> Search(string searchString);
        void AddMovie(string movieTitle, DateTime releaseDate, string releaseYear, List<int>? movieGenres);
        Movie SearchByID(int movieID);
        bool GetValidMovie(int movieID);
        IEnumerable<Movie> SearchByReleaseYear(int releaseYear);
        IEnumerable<Genre> GetGenres();
        bool UpdateMovieTitle(int movieID, string movieTitle);
        bool UpdateMovieReleaseYear(int movieID, int releaseYear);
        bool UpdateMovieReleaseDate(int movieID, DateTime releaseDate);
        bool AddMovieGenres(int movieID, List<int> movieGenres);
        bool UpdateMovieGenres(int movieID, List<int> movieGenres);
        bool DeleteMovieGenre(int movieID, int movieGenreGenreID);
        void DisplayMovieDetails(int movieID);
        void DeleteMovie(int movieID);

    }
}
