using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.ColorConverters.Original;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Q42.HueApi.ColorConverters;

namespace presence_lighting.Helpers
{
    class HueHelper
    {

        private static LocalHueClient hueClient;
        private static string _hueKey;
        private static string _userIp;

        public static void Initialise(string hueAppKey, string userIp)
        {
            _hueKey = hueAppKey;
            _userIp = userIp;
            hueClient = GetClient().Result;
        }

        public static void UpdatePresence(string presence)
        {
            /*
             * For now, set colour as red for a brief period and then
             * return to colour as previous.
             */

            var lightState = hueClient.GetLightAsync("1").Result.State;

            var command = new LightCommand();
            command.TurnOn().SetColor(new RGBColor("cf190c")); // teams red
            command.Alert = Alert.Multiple;

            hueClient.SendCommandAsync(command, new List<string> { "1" });

        }

        static async Task<LocalHueClient> GetClient()
        {
            LocalHueClient client = null;

            string ip = await GetOrFindIp();

            if (string.IsNullOrEmpty(ip))
                return null;

            client = new LocalHueClient(ip);
            client.Initialize(_hueKey); // Get app key from config

            if (!client.IsInitialized)
            {
                return null;
            }

            return client;
        }

        static async Task<string> GetOrFindIp()
        {
            string ip = _userIp; // get ip from config

            if (string.IsNullOrEmpty(ip))
            {
                IBridgeLocator locator = new HttpBridgeLocator();
                IEnumerable<LocatedBridge> bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));

                if (bridges.Any())
                {
                    ip = bridges.First().IpAddress;
                    Console.WriteLine($"Bridge has been found at {ip}");
                }
                else
                {
                    Console.WriteLine($"Could not find a hue bridge, your config may not contain the correct IP addresss for your bridge");
                    return null;
                }
            }

            return ip;
        }

    }
}