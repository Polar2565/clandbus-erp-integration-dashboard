# Security policy

This repository is a technical proof of concept. Do not connect it to production ERP data without a security and architecture review.

## Configuration

- Never commit ERP usernames, passwords, session cookies, customer data, or private instance URLs.
- Copy `appsettings.Development.example.json` to an ignored `appsettings.Development.json`, or use .NET user secrets/environment variables.
- Use a trusted HTTPS certificate. Certificate validation must not be disabled.
- Use a dedicated least-privilege Acumatica account and a non-production tenant for testing.

Example with .NET user secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "Acumatica:BaseUrl" "https://your-instance.example/"
```

Credentials are entered at runtime and must not be stored by the frontend or written to logs.

## Known prototype limitation

The current service holds one ERP session in memory. It is appropriate for a single-user technical demonstration only. A multi-user deployment must isolate sessions per authenticated application user and add authorization, audit controls, rate limiting, and secret management.

## Reporting

Please report a suspected exposure privately to the repository owner. Do not include credentials or customer data in a public issue.
