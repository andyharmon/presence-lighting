using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace presence_lighting.Helpers
{
    class HueHelper
    {

        private static LocalHueClient hueClient;
        private static string _hueKey;
        private static string _userIp;

        public static void Initialise(string hueAppKey, string userIp)
        {
            hueClient = GetClient().Result;
            _hueKey = hueAppKey;
            _userIp = userIp;
        }

        static async Task<LocalHueClient> GetClient()
        {
            LocalHueClient client = null;

            string ip = await GetOrFindIp();

            if (string.IsNullOrEmpty(ip))
                return null;

            client = new LocalHueClient(ip);
            client.Initialize(_hueKey); // Get app key from config

            if(!client.IsInitialized)
            {
                return null;
            }

            return client;
        }

        static async Task<string> GetOrFindIp()
        {
            string ip = _userIp; // get ip from config

            if(string.IsNullOrEmpty(ip))
            {
                IBridgeLocator locator = new HttpBridgeLocator();
                IEnumerable<LocatedBridge> bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));

                if(bridges.Any())
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