# presence-lighting

A simple console app that sets Hue lights based on my availability in Outlook

## Getting Started

Before you can run the application, you will need to create an application ID within an Azure Active Directory to make use of the Microsoft Identity Platform. [This guide from Microsoft should guide you through this process](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app).

This application uses the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows#secret-manager) provided through .NET Core with the following syntax:

    {
      "appId": <APPLICATION_ID_FROM_AZURE_AD>,
      "scopes": "User.Read;Calendars.Read;Presence.Read"
      "hueAppId": "<USERNAME_FROM_HUE_API>",
      "hueBridgeIp": "<HUE_BRIDGE_IP_ADDRESS>"
    }

For the Presence endpoint to be accessed, the scope **requires** the `Presence.Read` scope. Note that this scope is only supported by School or Work Microsoft accounts. For more information about the Presence Graph API endpoint, see [Microsoft's Documentation](https://docs.microsoft.com/en-us/graph/api/presence-get?view=graph-rest-beta&tabs=http).

To access your Hue Bridge, see [Philips Hue Developer Documentation](https://developers.meethue.com/develop/get-started-2/).

## Credits

This project makes use of the open source [Q42.HueApi](https://github.com/Q42/Q42.HueApi) package from [Q42](https://www.q42.nl/).
