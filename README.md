# EMMA Solution – API

ASP.NET Core 8 Web API + EF Core (SQL Server). It also serves the built web app from `wwwroot`,
so production is a single site: https://emmasolution.runasp.net (API under `/api`, Swagger at `/swagger`).

## Configuration

`appsettings.json` holds non-secret settings. Secrets live in `appsettings.Production.json`
(git-ignored, deployed with the app):

| Key | Purpose |
| --- | --- |
| `ConnectionStrings:Default` | SQL Server connection string |
| `SMTP:EmailAddress` / `SMTP:EmailPassword` | Gmail account used for notification emails |
| `Imgbb:ApiKey` | imgbb key for company logos |
| `Jwt:Key` | JWT signing key |
| `AppUrl` | Public URL used in email links |
| `AllowedCountries` | Address country suffixes accepted for loads (launch: `BiH`) |
| `Cors:Origins` | Extra frontend origins (e.g. Vercel) |

The database schema is created/updated automatically on startup (EF Core migrations in `Migrations/`).
From outside the hosting network use `db82714.public.databaseasp.net` as server name.

## Run locally

```bash
ConnectionStrings__Default="Server=db82714.public.databaseasp.net;..." ASPNETCORE_URLS=http://localhost:5099 dotnet run
```

## Deploy

1. `npm run build` in `Emma.Solution.App`, copy `dist/*` to `wwwroot/`
2. `dotnet publish -c Release -o publish`
3. Publish the folder with the Web Deploy profile (`*.publishSettings`, git-ignored)

New schema changes: `dotnet ef migrations add <Name>`; they are applied on the next start.
