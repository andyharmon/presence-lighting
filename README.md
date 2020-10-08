# presence-lighting

A simple console app that sets Hue lights based on my availability in Outlook

## Getting Started

Before you can run the application, you will need to create an application ID within an Azure Active Directory to make use of the Microsoft Identity Platform. [This guide from Microsoft should guide you through this process](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app).

This application uses the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows#secret-manager) provided through .NET Core with the following syntax:

    {
      "appId": <APPLICATION_ID>,
      "scopes": "User.Read;Calendars.Read;Presence.Read"
    }

For the Presence endpoint to be accessed, the scope **requires** the `Presence.Read` scope. Note that this scope is only supported by School or Work Microsoft accounts. For more information about the Presence Graph API endpoint, see [Microsoft's Documentation](https://docs.microsoft.com/en-us/graph/api/presence-get?view=graph-rest-beta&tabs=http)
