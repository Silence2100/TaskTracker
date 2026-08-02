## Local secrets

The application uses .NET User Secrets for sensitive local configuration.

Required secrets:

- `ConnectionStrings:PostgresConnection`
- `Jwt:SecretKey`

Set them with:

dotnet user-secrets set "ConnectionStrings:PostgresConnection" "..." --project crud/TaskTracker.Api
dotnet user-secrets set "Jwt:SecretKey" "..." --project crud/TaskTracker.Api
