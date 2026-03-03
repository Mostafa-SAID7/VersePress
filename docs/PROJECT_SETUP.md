# Project Setup Guide

## Prerequisites

### Required Software
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (9.0 or later)
- [SQL Server](https://www.microsoft.com/sql-server) or SQL Server LocalDB
- [Git](https://git-scm.com/)

### Recommended IDEs
- [Visual Studio 2024](https://visualstudio.microsoft.com/) (Community, Professional, or Enterprise)
- [Visual Studio Code](https://code.visualstudio.com/) with C# extension
- [JetBrains Rider](https://www.jetbrains.com/rider/)

## Installation Steps

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/versepress.git
cd versepress
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Configure Database

#### Option A: SQL Server LocalDB (Recommended for Development)

The default connection string uses LocalDB:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VersePressDb;Trusted_Connection=true;"
}
```

#### Option B: SQL Server

Update `src/VersePress.Web/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=VersePressDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
}
```

### 4. Apply Database Migrations

```bash
dotnet ef database update --project src/VersePress.Infrastructure --startup-project src/VersePress.Web
```

Or run the application (migrations apply automatically in development):
```bash
dotnet run --project src/VersePress.Web
```

### 5. Configure Email (Optional)

Update `src/VersePress.Web/appsettings.json`:
```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderName": "VersePress",
  "EnableSsl": true,
  "Username": "your-email@gmail.com",
  "Password": "your-app-password"
}
```

**Note**: For Gmail, use an [App Password](https://support.google.com/accounts/answer/185833).

### 6. Run the Application

```bash
dotnet run --project src/VersePress.Web
```

Access the application at: `https://localhost:5001`

## Default Accounts

### Admin Account
- **Email**: admin@versepress.com
- **Password**: Admin@123

### Author Accounts
- **Email**: john.doe@versepress.com / **Password**: Author@123
- **Email**: jane.smith@versepress.com / **Password**: Author@123

## Development Workflow

### Building the Solution

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Watch Mode (Auto-rebuild)

```bash
dotnet watch run --project src/VersePress.Web
```

### Creating Migrations

```bash
dotnet ef migrations add MigrationName --project src/VersePress.Infrastructure --startup-project src/VersePress.Web
```

### Updating Database

```bash
dotnet ef database update --project src/VersePress.Infrastructure --startup-project src/VersePress.Web
```

## IDE Setup

### Visual Studio 2024

1. Open `VersePress.sln`
2. Set `VersePress.Web` as startup project
3. Press F5 to run

### Visual Studio Code

1. Open the project folder
2. Install recommended extensions:
   - C# Dev Kit
   - C#
   - SQL Server (mssql)
3. Press F5 to run (or use terminal commands)

### JetBrains Rider

1. Open `VersePress.sln`
2. Set `VersePress.Web` as startup project
3. Press Shift+F10 to run

## Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to database

**Solutions**:
- Verify SQL Server is running
- Check connection string
- Ensure LocalDB is installed (comes with Visual Studio)
- Try: `sqllocaldb start mssqllocaldb`

### Migration Issues

**Problem**: Migrations fail to apply

**Solutions**:
- Delete the database and recreate: `dotnet ef database drop`
- Check for pending migrations: `dotnet ef migrations list`
- Ensure Infrastructure project references are correct

### Port Already in Use

**Problem**: Port 5001 is already in use

**Solutions**:
- Change port in `launchSettings.json`
- Kill process using the port
- Use different profile

### NuGet Package Restore Fails

**Problem**: Cannot restore packages

**Solutions**:
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Check internet connection
- Verify NuGet sources: `dotnet nuget list source`

## Environment Variables

For production, use environment variables instead of appsettings:

```bash
# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection="your-connection-string"
$env:EmailSettings__Username="your-email"
$env:EmailSettings__Password="your-password"

# Linux/Mac
export ConnectionStrings__DefaultConnection="your-connection-string"
export EmailSettings__Username="your-email"
export EmailSettings__Password="your-password"
```

## Next Steps

- Read [FEATURES.md](FEATURES.md) to understand available features
- Check [STRUCTURE.md](STRUCTURE.md) to understand the codebase
- See [CONTRIBUTING.md](CONTRIBUTING.md) to start contributing
- Review [DEPLOYMENT.md](DEPLOYMENT.md) for production deployment

## Support

If you encounter issues:
1. Check existing [GitHub Issues](https://github.com/yourusername/versepress/issues)
2. Create a new issue with details
3. Contact: support@versepress.com
