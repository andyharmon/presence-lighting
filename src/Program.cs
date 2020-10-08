using presence_lighting.Authentication;
using presence_lighting.Helpers;
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

            var azureAppId = appConfig["appId"];
            var scopesString = appConfig["scopes"];
            var scopes = scopesString.Split(";");
            var hueAppId = appConfig["hueAppId"];
            var hueBridgeIp = appConfig["hueBridgeIp"];

            // Initialise the HueHelper
            HueHelper.Initialise(hueAppId, hueBridgeIp);

            // initalise the auth provider with values from appsettings
            var authProvider = new DeviceCodeAuthProvider(azureAppId, scopes);

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
                string previousPresence = string.Empty; // used to detect changes to presence
                string currentPresence;
                do
                {
                    while (Console.KeyAvailable == false)
                    {
                        currentPresence = GetPresence();

                        // if currentPresence is different to previousPresence,
                        // tell the user and make changes in the Hue API

                        if (currentPresence != previousPresence)
                        {
                            Console.WriteLine($"User is now {currentPresence}");
                            HueHelper.UpdatePresence(currentPresence);
                        }
                        previousPresence = currentPresence;
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
