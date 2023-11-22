using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace MovieApp.Services
{
	public class Helper
	{

        //Get a year in YYYY format from a user.
        public static string GetYear(string prompt)
        {
            var yearPattern = @"^(19|20)\d{2}$";
            var thisYear = DateTime.UtcNow.Year;

            while (true)
            {
                Console.WriteLine(prompt);
                var userInput = Console.ReadLine();
                int userYear = 0;

                try
                {
                    userYear = Int32.Parse(userInput);

                }
                catch (FormatException)
                {
                    Console.WriteLine("\nPlease enter a valid year in the " +
                        "following format: YYYY (e.g., 1999).\n");
                    continue;
                }
                if (Regex.Match(userInput, yearPattern).Success && userYear >= 1900 && (userYear < thisYear + 1))
                {
                    return userInput;
                }
                else
                {
                    Console.WriteLine("\nPlease enter a valid year between 1900 and " +
                        $"{thisYear} in the following format: YYYY (e.g., 1999).\n");
                }

            }

        }

        //Convert a string into Title Case, in which the first letter of each word is capitalized.
        public static string ConvertTitle(string text)
        {
            string finalText = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i == 0 || text[i - 1] == ' ') //capitalizes first letter in string and any letter following a blank space.
                    finalText += Char.ToUpper(text[i]);
                else
                    finalText += Char.ToLower(text[i]);
            }
            return finalText;
        }

        //Get an integer within a certain range.
        public static int GetIntInRange(string prompt, int bottom, int top)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine(prompt);
                    var userNumber = Convert.ToInt32(Console.ReadLine());
                    if (userNumber < bottom || userNumber > top)
                    {
                        Console.WriteLine($"Please enter a valid number.");
                        continue;
                    }
                    return userNumber;
                }
                catch
                {
                    Console.WriteLine($"Please enter a valid number between {bottom} and {top}.");
                }
            }
        }

        public static char YesNo(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                var userEntry = Console.ReadLine();
                try
                {
                    var userAnswer = Char.ToUpper(Convert.ToChar(userEntry));
                    if (userAnswer == 'Y' || userAnswer == 'N')
                        return userAnswer;
                }
                catch
                {
                    Console.WriteLine("You must enter either 'Y' for yes or 'N' for no.");
                }
            }
        }

    }
}