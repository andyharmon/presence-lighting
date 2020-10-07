using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace presence_lighting.Graph
{
    class GraphHelper
    {
        private static GraphServiceClient graphServiceClient;
        public static void Initialise(IAuthenticationProvider authenticationProvider)
        {
            graphServiceClient = new GraphServiceClient(authenticationProvider);
        }

        public static async Task<User> GetMeAsync()
        {
            try
            {
                // GET /me
                return await graphServiceClient.Me.Request().GetAsync();
            }
            catch (ServiceException e)
            {
                Console.WriteLine($"Error getting signed-in user: {e.Message}");
                return null;
            }
        }

        public static async Task<Presence> GetMePresenceAsync()
        {
            try
            {
                return await graphServiceClient.Me.Presence
                    .Request()
                    .GetAsync();
            }
            catch (ServiceException e)
            {
                Console.WriteLine($"Error getting user presence: {e.Message}");
                return null;
            }
        }
    }
}