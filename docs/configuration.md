# Configuration

This document lists configuration keys the SpendWise API expects, which are secrets and where to store them.

Secrets (store in user-secrets or environment variables in production):
- Jwt__Key (secret signing key, minimum 32 characters) - secret
- ConnectionStrings__DefaultConnection - secret

Non-secret settings (appsettings.json or appsettings.Development.json):
- Jwt__Issuer
- Jwt__Audience
- Jwt__ExpiryMinutes
- Cors__AllowedOrigins__0 (repeatable array) - browser origins allowed
- Budgets__TimeZoneId
- Serilog__MinimumLevel__Default
- Serilog__WriteTo__0__Name, etc.

Environment variables mapping examples (use __ to separate sections):
- Jwt__Key
- ConnectionStrings__DefaultConnection
- Cors__AllowedOrigins__0
- Budgets__TimeZoneId

Do NOT store secrets in source control.
