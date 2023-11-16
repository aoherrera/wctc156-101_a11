using MovieLibraryEntities.Models;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection.PortableExecutable;
using MovieLibraryEntities.Dao;

namespace MovieApp.Services;

/// <summary>
///     You would need to inject your interfaces here to execute the methods in Invoke()
///     See the commented out code as an example
/// </summary>
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
            Console.WriteLine("\n1.) Search for movie\n2.) Add a movie\n3.) List movies\n4.) Exit\n");
            choice = Console.ReadLine();

            if (choice == "1")
            {
                {
                    Console.WriteLine("Search a movie title: ");
                    var user_movie = Console.ReadLine().ToUpper();
                    var movies = _repository.Search(user_movie);

                    if (movies.Any())
                    {
                        foreach (Movie movie in movies)
                        {
                            Console.WriteLine($"Movie ID: {movie.Id}");
                            Console.WriteLine($"Title (Release Year): {movie.Title}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No movies matched the text entered.");
                    }
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine("Please enter the movie title:");
                var userMovie = Helper.ConvertTitle(Console.ReadLine());

                var releaseYear = Helper.GetYear("Please enter the release year:");

                var releaseMonth = Helper.GetIntInRange("Please enter the month (number) the movie was released", 1, 12);

                int releaseDay;
                if (releaseMonth == 2)
                    releaseDay = Helper.GetIntInRange("Please enter the day the movie was released", 1, 28);
                else
                    releaseDay = Helper.GetIntInRange("Please etner the day the movie was released", 1, 28);

                string movieRelease = releaseYear + "-" + Convert.ToString(releaseMonth) + "-" + Convert.ToString(releaseDay);
                var movieReleaseDate = Convert.ToDateTime(movieRelease);

                _repository.AddMovie(userMovie, movieReleaseDate, releaseYear);           

            }
            else if (choice == "3")
            {
                var movies = _repository.GetAll();
                foreach (Movie movie in movies)
                {
                    Console.WriteLine($"Movie ID: {movie.Id}");
                    Console.WriteLine($"Title (Release Year): {movie.Title}");
                    Console.WriteLine();
                }
            }
                else if (choice == "4")
                        break;

                    else
                        Console.WriteLine("Please enter a valid selection.\n");
        }
        while (choice != "4");
    }
}
