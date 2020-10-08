using presence_lighting.Authentication;
using presence_lighting.Graph;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace presence_lighting
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-- Presence Lighting Console App -- ");

            var appConfig = LoadAppSettings();
            if (appConfig == null)
            {
                Console.WriteLine("Missing or invalid appsettings.json");
                return;
            }

            var appId = appConfig["appId"];
            var scopesString = appConfig["scopes"];
            var scopes = scopesString.Split(";");

            // initalise the auth provider with values from appsettings
            var authProvider = new DeviceCodeAuthProvider(appId, scopes);

            // request a token to sign the user in
            var accessToken = authProvider.GetAccessToken().Result;

            // Initalise the MS Graph Client
            GraphHelper.Initialise(authProvider);

            // Get the signed in user
            var user = GraphHelper.GetMeAsync().Result;
            Console.WriteLine($"Welcome {user.DisplayName}!\n");

            int choice = -1;

            while (choice < 0)
            {
                Console.WriteLine("Please choose one of the following options:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Display access token");
                Console.WriteLine("2. Display User's current presence");
                Console.WriteLine("3. Monitor Presence");

                try
                {
                    choice = int.Parse(Console.ReadLine());
                }
                catch (System.FormatException)
                {

                    // set choice to an invalid value
                    choice = -1;
                }

                switch (choice)
                {
                    case 0:
                        // exit the program
                        Console.WriteLine("Goodbye...");
                        break;

                    case 1:
                        // display access token
                        Console.WriteLine($"Access token: {accessToken}\n");
                        choice = -1;
                        break;

                    case 2:
                        // Display user's current presence
                        Console.WriteLine($"{user.DisplayName} is currently {GetPresence()}");
                        choice = -1;
                        break;

                    case 3:
                        // Monitor presence
                        Console.WriteLine($"{user.DisplayName}'s presence will be monitored");
                        MonitorPresence();
                        choice = -1;
                        break;

                    default:
                        Console.WriteLine("Invalid Choice! Please try again");
                        choice = -1;
                        break;
                }

            }

            static string GetPresence()
            {
                var presence = GraphHelper.GetMePresenceAsync().Result;
                if (presence != null)
                {
                    return presence.Availability;
                }
                else
                {
                    return "not found";
                }
            }

            static void MonitorPresence()
            {
                ConsoleKeyInfo cki;
                do
                {
                    Console.WriteLine("\nDisplaying user presence, press the 'x' key to quit.");
                    while (Console.KeyAvailable == false)
                    {
                        Console.WriteLine($"User is currently {GetPresence()}");
                        Thread.Sleep(5000); // Loop every 5 second until input is entered.
                    }

                    cki = Console.ReadKey(true);
                    Console.WriteLine("Exiting...\n");
                } while (cki.Key != ConsoleKey.X);
            }


            static IConfigurationRoot LoadAppSettings()
            {
                var appConfig = new ConfigurationBuilder()
                    .AddUserSecrets<Program>()
                    .Build();

                if (string.IsNullOrEmpty(appConfig["appId"]) ||
                    string.IsNullOrEmpty(appConfig["scopes"]))
                {
                    return null;
                }

                return appConfig;
            }
        }
    }
}
