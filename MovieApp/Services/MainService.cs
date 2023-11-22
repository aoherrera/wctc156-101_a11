using MovieLibraryEntities.Models;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection.PortableExecutable;
using MovieLibraryEntities.Dao;
using System.Collections.Generic;

namespace MovieApp.Services;

public class MainService : IMainService
{
    private readonly IRepository _repository;

    public MainService(IRepository repository)
    {
        _repository = repository;
    }

    public void Invoke()
    {
        string choice;
        do
        {
            Console.WriteLine("What action would you like to perform?");
            Console.WriteLine("\n1.) Search for movie by title\n2.) Search for movie by release year\n3.) Add a movie\n4.) List movies\n5.) Update movie\n6.) Delete movie\n7.) Exit");
            choice = Console.ReadLine();
            Console.WriteLine();

            if (choice == "1")
            {
                {
                    string userMovie;
                    do
                    {
                        Console.WriteLine("Search a movie title: ");
                        userMovie = Console.ReadLine();
                        if (String.IsNullOrWhiteSpace(userMovie))
                        {
                            Console.WriteLine("Error: You must enter a valid movie title.");
                            break;
                        }
                        else
                        {
                            var movies = _repository.Search(userMovie);
                            if (movies.Any())
                            {
                                for (int i = 0; i < movies.Count(); i++)
                                {
                                    var movieGenres = movies.ElementAt(i)?.MovieGenres ?? new List<MovieGenre>();

                                    if (i <= (movies.Count() - 1))
                                    {
                                        Console.WriteLine($"\nMovie ID: {movies.ElementAt(i).Id}");
                                        Console.WriteLine($"Title (Release Year): {movies.ElementAt(i).Title}");
                                    }
                                    else if (i == (movies.Count() - 1) && movieGenres.Count == 0)
                                    {
                                        Console.WriteLine($"\nMovie ID: {movies.ElementAt(i).Id}");
                                        Console.WriteLine($"Title (Release Year): {movies.ElementAt(i).Title}\n");
                                    }

                                    //display genre(s)
                                    if (movieGenres.Count() != 0)
                                    {
                                        Console.Write("Genre(s): ");
                                        for (int j = 0; j < movieGenres.Count(); j++)
                                        {
                                            if (j != (movieGenres.Count() - 1))
                                            {
                                                Console.Write($"{movieGenres.ElementAt(j).Genre.Name}, ");
                                            }
                                            else
                                                Console.WriteLine($"{movieGenres.ElementAt(j).Genre.Name}\n");
                                        }
                                    }

                                }
                            }
                            else
                            {
                                Console.WriteLine("\nNo movies matched the text entered.");
                                Console.WriteLine("\nNote: you may only search by movie title.\nIf you would like to search by release year, please select another option.\n");
                            }
                        }
                    } while (String.IsNullOrWhiteSpace(userMovie));
                }
            }
            else if (choice == "2")
            {
                string userYear;
                do
                {
                    userYear = Helper.GetYear("Enter the movie release year to search.");
                    int releaseYear;
                    try
                    {
                        releaseYear = Convert.ToInt32(userYear);
                    }
                    catch
                    {
                        Console.WriteLine("Error: You must enter a valid year.");
                        break;
                    }
                    var movies = _repository.SearchByReleaseYear(releaseYear);
                    if (movies.Any())
                    {
                        for (int i = 0; i < movies.Count(); i++)
                        {
                            var movieGenres = movies.ElementAt(i)?.MovieGenres ?? new List<MovieGenre>();

                            if (i <= (movies.Count() - 1))
                            {
                                Console.WriteLine($"\nMovie ID: {movies.ElementAt(i).Id}");
                                Console.WriteLine($"Title (Release Year): {movies.ElementAt(i).Title}");
                            }
                            else if (i == (movies.Count() - 1) && movieGenres.Count == 0)
                            {
                                Console.WriteLine($"\nMovie ID: {movies.ElementAt(i).Id}");
                                Console.WriteLine($"Title (Release Year): {movies.ElementAt(i).Title}\n");
                            }

                            //display genre(s)
                            if (movieGenres.Count() != 0)
                            {
                                Console.Write("Genre(s): ");
                                for (int j = 0; j < movieGenres.Count(); j++)
                                {
                                    if (j != (movieGenres.Count() - 1))
                                    {
                                        Console.Write($"{movieGenres.ElementAt(j).Genre.Name}, ");
                                    }
                                    else
                                        Console.WriteLine($"{movieGenres.ElementAt(j).Genre.Name}\n");
                                }
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("\nNo movies matched the text entered.");
                        Console.WriteLine("\nNote: you may only search by movie title.\nIf you would like to search by release year, please select another option.\n");
                    }
                } while (String.IsNullOrWhiteSpace(userYear));
            }
            else if (choice == "3")
            {
                string userMovie;
                do
                {
                    Console.WriteLine("Please enter the movie title:");
                    userMovie = Helper.ConvertTitle(Console.ReadLine());
                    if (String.IsNullOrWhiteSpace(userMovie))
                    {
                        Console.WriteLine("Error: You must enter a valid movie title.\n");
                        break;
                    }
                    else
                    {
                        var releaseYear = Helper.GetYear("Please enter the release year:");

                        var releaseMonth = Helper.GetIntInRange("Please enter the month (number) the movie was released", 1, 12);

                        int releaseDay;

                        //allow for up to 29 days for February.
                        if (releaseMonth == 2)
                            releaseDay = Helper.GetIntInRange("Please enter the day the movie was released", 1, 29);
                        //allow for up to 30 days for months of April, June, September and November.
                        else if (releaseMonth == 4 || releaseMonth == 6 || releaseMonth == 9 || releaseMonth == 11)
                            releaseDay = Helper.GetIntInRange("Please enter the day the movie was released", 1, 30);
                        //allow up to 31 days for the other months.
                        else
                            releaseDay = Helper.GetIntInRange("Please enter the day the movie was released", 1, 31);

                        string movieRelease = releaseYear + "-" + Convert.ToString(releaseMonth) + "-" + Convert.ToString(releaseDay);
                        var movieReleaseDate = Convert.ToDateTime(movieRelease);

                        //add genre(s) to movie.
                        var addGenre = Helper.YesNo($"Would you like to enter genre(s) for {userMovie} (Y/N)?");
                        if (addGenre == 'N')
                        {
                            _repository.AddMovie(userMovie, movieReleaseDate, releaseYear, null);
                        }
                        else
                        {
                            List<int> userGenres = new List<int>();

                            var genres = _repository.GetGenres();
                            foreach (var genre in genres)
                            {
                                if (genre.Id > 10)
                                {
                                    Console.WriteLine($"ID: {genre.Id} Genre: {genre.Name}");
                                }
                                else
                                    Console.WriteLine($"ID: 0{genre.Id} Genre: {genre.Name}");
                            }
                            var userGenre = Helper.GetIntInRange("\nPlease enter the ID for the genre you would like to add:", (int)genres.Min(x => x.Id), (int)genres.Max(x => x.Id));
                            userGenres.Add(userGenre);

                            char addAnotherGenre;
                            do
                            {
                                addAnotherGenre = Helper.YesNo("Would you like to add another genre (Y/N)?");
                                if (addAnotherGenre == 'Y')
                                {
                                    userGenre = Helper.GetIntInRange("\nPlease enter the ID for the genre you would like to add:", (int)genres.Min(x => x.Id), (int)genres.Max(x => x.Id));
                                    if (userGenres.Contains(userGenre))
                                    {
                                        var duplicateGenre = _repository.GetGenres().FirstOrDefault(x => x.Id == userGenre);
                                        Console.WriteLine($"Error: Genre ID {duplicateGenre.Id} - {duplicateGenre.Name} is already associated with this movie.");
                                        continue;
                                    }

                                    userGenres.Add(userGenre);
                                }
                                else
                                {
                                    _repository.AddMovie(userMovie, movieReleaseDate, releaseYear, userGenres);
                                    break;
                                }
                            } while (addAnotherGenre == 'Y');
                        }


                    }
                } while (String.IsNullOrWhiteSpace(userMovie));

            }
            else if (choice == "4")
            {
                var seeAll = Helper.YesNo("Would you like to view all movie records (Y/N)?");
                if (seeAll == 'Y')
                {
                    var movies = _repository.GetAll();
                    for (int i = 0; i < movies.Count(); i++)
                    {
                        var movieGenres = movies.ElementAt(i)?.MovieGenres ?? new List<MovieGenre>();

                        if (i <= (movies.Count() - 1))
                        {
                            Console.WriteLine($"\nMovie ID: {movies.ElementAt(i).Id}");
                            Console.WriteLine($"Title (Release Year): {movies.ElementAt(i).Title}");
                        }
                        else if (i == (movies.Count() - 1) && movieGenres.Count == 0)
                        {
                            Console.WriteLine($"\nMovie ID: {movies.ElementAt(i).Id}");
                            Console.WriteLine($"Title (Release Year): {movies.ElementAt(i).Title}\n");
                        }

                        //display genre(s)
                        if (movieGenres.Count() != 0)
                        {
                            Console.Write("Genre(s): ");
                            for (int j = 0; j < movieGenres.Count(); j++)
                            {
                                if (j != (movieGenres.Count() - 1))
                                {
                                    Console.Write($"{movieGenres.ElementAt(j).Genre.Name}, ");
                                }
                                else
                                    Console.WriteLine($"{movieGenres.ElementAt(j).Genre.Name}\n");
                            }
                        }
                    }
                }
                else
                {
                    var amountToSee = Helper.GetIntInRange("How many movie records would you like to see (sorted alphabetically)?", 0, _repository.GetAll().Count());
                    var movies = _repository.GetTopMovies(amountToSee);
                    for (int i = 0; i < movies.Count(); i++)
                    {
                        var movieGenres = movies.ElementAt(i)?.MovieGenres ?? new List<MovieGenre>();

                        if (i <= (movies.Count() - 1))
                        {
                            Console.WriteLine($"\nMovie ID: {movies.ElementAt(i).Id}");
                            Console.WriteLine($"Title (Release Year): {movies.ElementAt(i).Title}");
                        }
                        else if (i == (movies.Count() - 1) && movieGenres.Count == 0)
                        {
                            Console.WriteLine($"\nMovie ID: {movies.ElementAt(i).Id}");
                            Console.WriteLine($"Title (Release Year): {movies.ElementAt(i).Title}\n");
                        }

                        //display genre(s)
                        if (movieGenres.Count() != 0)
                        {
                            Console.Write("Genre(s): ");
                            for (int j = 0; j < movieGenres.Count(); j++)
                            {
                                if (j != (movieGenres.Count() - 1))
                                {
                                    Console.Write($"{movieGenres.ElementAt(j).Genre.Name}, ");
                                }
                                else
                                    Console.WriteLine($"{movieGenres.ElementAt(j).Genre.Name}\n");
                            }
                        }
                    }
                }
            }
            else if (choice == "5")
            {
                var knowsID = Helper.YesNo("Do you know the movie ID for the movie ID you would like to update (Y/N)?");
                while (knowsID == 'N')
                {
                    Console.WriteLine("Search a movie title: ");
                    var user_movie = Console.ReadLine().ToUpper();
                    var movies = _repository.Search(user_movie);

                    if (movies.Any())
                    {
                        for (int i = 0; i < movies.Count(); i++)
                        {
                            Console.WriteLine($"Movie ID: {movies.ElementAt(i).Id}");
                            Console.WriteLine($"Title (Release Year): {movies.ElementAt(i).Title}.");

                            if (i != movies.Count() - 1)
                            {
                                Console.WriteLine();
                            }
                        }
                        var searchAgain = Helper.YesNo("\nWould you like to search again (Y/N)?");
                        if (searchAgain == 'Y')
                            continue;
                        else
                            knowsID = 'Y';
                    }
                    else
                    {
                        Console.WriteLine("No movies matched the text entered.");
                        var searchAgain = Helper.YesNo("\nWould you like to search again (Y/N)?");
                        if (searchAgain == 'Y')
                            continue;
                        else
                            break;
                    }
                }
                //flag variable to check if movie has been updated.
                bool movieUpdated = false;
                do
                {
                    Console.WriteLine("Enter the movie ID for the movie you woud like to update.");
                    var userMovie = Console.ReadLine();
                    int movieID;
                    try
                    {
                        movieID = Convert.ToInt32(userMovie);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Error: you must enter a valid ID.");
                        break;
                    }
                    if (_repository.GetValidMovie(movieID) != true)
                    {
                        Console.WriteLine("Error: You must enter a valid movie ID.");
                        break;
                    }
                    int changeChoice;
                    _repository.DisplayMovieDetails(movieID);
                    do
                    {
                        Console.WriteLine("\nWhich element of the selected movie would you like to update?");
                        changeChoice = Helper.GetIntInRange("\n1.) Movie Title\n2.) Release Year\n3.) Release Date\n4.) Genre(s)\n5.) Never mind", 1, 5);
                        if (changeChoice == 5)
                        {
                            //set movieUpdated flag to true to break out of
                            //update workflow completely.
                            movieUpdated = true;
                            break;
                        }
                        //update movie title
                        else if (changeChoice == 1)
                        {
                            Console.WriteLine("Please enter the new movie title:");
                            var newTitle = Console.ReadLine();
                            if (String.IsNullOrWhiteSpace(newTitle))
                            {
                                Console.WriteLine("Error: You must enter a valid movie title:");
                                //set movieUpdated flag to true to break out of
                                //update workflow completely.
                                movieUpdated = true;
                                break;
                            }
                            movieUpdated = _repository.UpdateMovieTitle(movieID, Helper.ConvertTitle(newTitle));
                        }
                        //update movie release year
                        else if (changeChoice == 2)
                        {
                            var newYear = Helper.GetYear("Please enter the new release year.");
                            movieUpdated = _repository.UpdateMovieReleaseYear(movieID, Convert.ToInt32(newYear));
                        }
                        else if (changeChoice == 3)
                        {
                            var releaseYear = Helper.GetYear("Please enter the release year:");

                            var releaseMonth = Helper.GetIntInRange("Please enter the month (number) the movie was released", 1, 12);

                            int releaseDay;

                            //allow for up to 29 days for February.
                            if (releaseMonth == 2)
                                releaseDay = Helper.GetIntInRange("Please enter the day the movie was released", 1, 29);
                            //allow for up to 30 days for months of April, June, September and November.
                            else if (releaseMonth == 4 || releaseMonth == 6 || releaseMonth == 9 || releaseMonth == 11)
                                releaseDay = Helper.GetIntInRange("Please enter the day the movie was released", 1, 30);
                            //allow up to 31 days for the other months.
                            else
                                releaseDay = Helper.GetIntInRange("Please enter the day the movie was released", 1, 31);

                            string movieRelease = releaseYear + "-" + Convert.ToString(releaseMonth) + "-" + Convert.ToString(releaseDay);
                            var movieReleaseDate = Convert.ToDateTime(movieRelease);
                            movieUpdated = _repository.UpdateMovieReleaseDate(movieID, movieReleaseDate);
                        }
                        else if (changeChoice == 4)
                        {
                            int genreChoice;
                            do
                            {
                                Console.WriteLine("\nWould you like to add or remove a genre?");
                                genreChoice = Helper.GetIntInRange("1.) Add Genre\n2.) Remove Genre\n3.) Never mind", 1, 3);
                                if (genreChoice == 3)
                                {
                                    //kick user out of entire update worklow.
                                    movieUpdated = true;
                                    break;
                                }
                                else if (genreChoice == 1)
                                {
                                    List<int> userGenres = new List<int>();

                                    var genres = _repository.GetGenres();
                                    foreach (var genre in genres)
                                    {
                                        if (genre.Id <= 9)
                                        {
                                            Console.WriteLine($"ID: 0{genre.Id} Genre: {genre.Name}");
                                        }
                                        else
                                            Console.WriteLine($"ID: {genre.Id} Genre: {genre.Name}");
                                    }
                                    var userGenre = Helper.GetIntInRange("\nPlease enter the ID for the genre you would like to add:", (int)genres.Min(x => x.Id), (int)genres.Max(x => x.Id));
                                    if (userGenres.Contains(userGenre) || _repository.SearchByID(movieID).MovieGenres.Any(x => x.Genre == _repository.GetGenres().FirstOrDefault(x => x.Id == userGenre)))
                                    {
                                        var duplicateGenre = _repository.GetGenres().FirstOrDefault(x => x.Id == userGenre);
                                        Console.WriteLine($"Error: Genre ID {duplicateGenre.Id} - {duplicateGenre.Name} is already associated with this movie.");
                                        continue;
                                    }
                                    userGenres.Add(userGenre);

                                    char addAnotherGenre;
                                    do
                                    {
                                        addAnotherGenre = Helper.YesNo("\nWould you like to add another genre (Y/N)?");
                                        if (addAnotherGenre == 'Y')
                                        {
                                            userGenre = Helper.GetIntInRange("\nPlease enter the ID for the genre you would like to add:", (int)genres.Min(x => x.Id), (int)genres.Max(x => x.Id));
                                            if (userGenres.Contains(userGenre) || _repository.SearchByID(movieID).MovieGenres.Any(x => x.Genre == _repository.GetGenres().FirstOrDefault(x => x.Id == userGenre)))
                                            {
                                                var duplicateGenre = _repository.GetGenres().FirstOrDefault(x => x.Id == userGenre);
                                                Console.WriteLine($"Error: Genre ID {duplicateGenre.Id} - {duplicateGenre.Name} is already associated with this movie.");
                                                continue;
                                            }
                                            userGenres.Add(userGenre);
                                        }
                                        else
                                        {
                                            _repository.AddMovieGenres(movieID, userGenres);
                                            movieUpdated = true;
                                            genreChoice = 3;
                                            break;
                                        }
                                    } while (addAnotherGenre == 'Y');
                                }
                                else if (genreChoice == 2)
                                {
                                    bool userDelete = false;
                                    do
                                    {
                                        var movie = _repository.SearchByID(movieID);
                                        var currentGenres = movie.MovieGenres ?? new List<MovieGenre>();
                                        if (!currentGenres.Any())
                                        {
                                            Console.WriteLine("Error: there are no genres associated with this movie.");
                                            movieUpdated = true;
                                            break;
                                        }
                                        else if (currentGenres.Any())
                                        {
                                            //create a menu of the current genres to display to the user. Allow the user to quit this workflow.
                                            //this option should be ICollection<MovieGenre>.Count() + 1 
                                            string menuCurrentGenres = "";

                                            for (int i = 0; i < currentGenres.Count(); i++)
                                            {
                                                menuCurrentGenres += $"{i + 1}.) {currentGenres.ElementAt(i).Genre.Name}\n";
                                                if (i == currentGenres.Count()-1)
                                                {
                                                    menuCurrentGenres += $"{i + 2}.) Never mind\n";
                                                }
                                            }
                                            Console.WriteLine("\nWhich genre would you like to delete?");
                                            var deleteGenre = Helper.GetIntInRange(menuCurrentGenres, 1, movie.MovieGenres.Count()+1);
                                            if (deleteGenre == currentGenres.Count() + 1)
                                            {
                                                userDelete = true;
                                                //kick user out of update workflow.
                                                movieUpdated = true;
                                                break;
                                            }
                                            var userVerification = Helper.YesNo($"Are you sure you want to delete {currentGenres.ElementAt(deleteGenre - 1).Genre.Name} (Y/N)?");
                                            if (userVerification == 'Y')
                                            {
                                                movieUpdated = _repository.DeleteMovieGenre(Convert.ToInt32(movie.Id), deleteGenre);
                                            }

                                            else if (userVerification == 'N')
                                            {
                                                userDelete = true;
                                                genreChoice = 3;
                                                break;
                                            }
                                            if (currentGenres.Any())
                                            {
                                                var deleteAnother = Helper.YesNo("Would you like to delete another genre (Y/N)?");
                                                if (deleteAnother == 'Y')
                                                {
                                                    userDelete = false;
                                                    continue;
                                                }
                                                else
                                                {
                                                    genreChoice = 3;
                                                    movieUpdated = true;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                userDelete = true;
                                                genreChoice = 3;
                                                break;
                                            }
                                        }

                                    } while (!userDelete);
                                }

                            } while (genreChoice != 3);
                        }
                        //display changes to user.
                        _repository.DisplayMovieDetails(movieID);

                        var editAnother = Helper.YesNo("Would you like to edit another element (Y/N)?");
                        if (editAnother == 'Y')
                        {
                            movieUpdated = false;
                            continue;
                        }
                        else if (editAnother == 'N')
                        {
                            break;
                        }
                    } while (changeChoice != 5);

                } while (movieUpdated == false);
            }

            else if (choice == "6")
            {
                var knowsID = Helper.YesNo("Do you know the ID of the movie you would like to delete (Y/N)?");
                while (knowsID == 'N')
                {
                    Console.WriteLine("Search a movie title: ");
                    var user_movie = Console.ReadLine().ToUpper();
                    var movies = _repository.Search(user_movie);

                    if (movies.Any())
                    {
                        for (int i = 0; i < movies.Count(); i++)
                        {
                            _repository.DisplayMovieDetails(Convert.ToInt32(movies.ElementAt(i).Id));

                            if (i != movies.Count() - 1)
                            {
                                Console.WriteLine();
                            }
                        }
                        var searchAgain = Helper.YesNo("\nWould you like to search again (Y/N)?");
                        if (searchAgain == 'Y')
                            continue;
                        else
                            knowsID = 'Y';
                    }
                    else
                    {
                        Console.WriteLine("No movies matched the text entered.");
                        var searchAgain = Helper.YesNo("\nWould you like to search again (Y/N)?");
                        if (searchAgain == 'Y')
                            continue;
                        else
                            break;
                    }

                    //tracks if the user has changed their mind regarding the erasure of a movie.
                    bool stopDeletionWorkflow = false;
                    do
                    {
                        Console.WriteLine("Enter the movie ID for the movie you woud like to delete.");
                        var userMovie = Console.ReadLine();
                        int movieID;
                        try
                        {
                            movieID = Convert.ToInt32(userMovie);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Error: you must enter a valid ID.");
                            //kick user out of erasure workflow.
                            stopDeletionWorkflow = true;
                            break;
                        }
                        if (_repository.GetValidMovie(movieID) != true)
                        {
                            Console.WriteLine("Error: You must enter a valid movie ID.");
                            //kick user out of erasure workflow.
                            stopDeletionWorkflow = true;
                            break;
                        }
                        var movieToDelete = _repository.SearchByID(movieID);
                        var verifyDelete = Helper.YesNo($"Are you sure you want to delete {movieToDelete.Title} (Y/N)?");
                        if (verifyDelete == 'N')
                        {
                            Console.WriteLine("No changes have been made to the database.");
                            stopDeletionWorkflow = true;
                            break;
                        }
                        else if (verifyDelete == 'Y')
                        {
                            _repository.DeleteMovie(movieID);
                            Console.WriteLine($"{movieToDelete.Title} deleted from database.");
                            stopDeletionWorkflow = true;
                        }
                    } while (stopDeletionWorkflow == false);
                }

            }
            else if (choice == "7")
                break;

            else
                Console.WriteLine("Please enter a valid selection.\n");

        } while (choice != "7");
    }
}