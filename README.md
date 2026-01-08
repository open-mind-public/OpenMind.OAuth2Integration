## Scenario
**CRM Integration with Gmail and Calendar**

Applcation CRM lets sales representatives see their Gmail emails and Google Calendar events directly inside the CRM, without giving the CRM their (sale users) Google passwords.

1. User logs in to CRM
    - The sales rep opens the CRM dashboard.
2. CRM asks for permission to access Gmail/Calendar
    - The CRM redirects the user to Google’s OAuth2 authorization endpoint.
    - User sees a consent screen: *“CRM wants to read your emails and events”*.
3. User grants consent
    - Google issues an access token (and optionally a refresh token).
    - The access token has limited scope: e.g., `gmail.readonly`, `calendar.readonly`.
4. CRM calls Google APIs on behalf of the user
    - CRM uses the access token to fetch the user’s emails and calendar events.
    - Emails appear in the CRM dashboard, without ever knowing the user’s Google password.
5. Token expiration and refresh
    - When the access token expires, the CRM uses the refresh token to get a new one.
    - No user interaction is required for this.

## Setup Instructions

### 1. Google OAuth2 Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Enable Gmail API and Google Calendar API
4. Create OAuth 2.0 credentials:
   - Application type: Web application
   - Authorized redirect URIs: `https://localhost:7001/api/google/callback`
5. Note down the Client ID and Client Secret

### 2. Backend Configuration

1. Update `appsettings.json` with your Google OAuth2 credentials:
   ```json
   {
     "Google": {
       "ClientId": "YOUR_GOOGLE_CLIENT_ID",
       "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET",
       "RedirectUri": "https://localhost:7001/api/google/callback"
     }
   }
   ```

2. Ensure PostgreSQL connection string is correct in `appsettings.json`

### 3. Database Setup

The application uses Entity Framework Core migrations for database schema management.

```bash
cd src/backend/OpenMind.CRM.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../OpenMind.CRM.API
dotnet ef database update --startup-project ../OpenMind.CRM.API
```

### 4. Run the Application

#### Backend:
```bash
cd src/backend/OpenMind.CRM.API
dotnet restore
dotnet run
```

The API will be available at `https://localhost:7001`

#### Frontend:
```bash
cd src/frontend
npm install
npm start
```

The Angular app will be available at `http://localhost:4200`

## Usage

1. **Register/Login**: Create a new account or login with existing credentials
2. **Connect Google Account**: Click "Connect Google Account" to authorize Gmail and Calendar access
3. **View Dashboard**: Access your emails and calendar events directly in the CRM interface

## OAuth2 Flow

The application implements the Authorization Code flow:

1. User clicks "Connect Google Account"
2. Redirected to Google's authorization server
3. User grants permissions
4. Google redirects back with authorization code
5. Backend exchanges code for access and refresh tokens
6. Tokens are stored securely and used for API calls
7. Refresh tokens automatically handle token expiration

![img.png](img.png)

## License

This project is for educational and demonstration purposes.