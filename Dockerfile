FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build

WORKDIR /build

COPY . .

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim

WORKDIR /app

COPY --from=build /build/out .

CMD ["dotnet", "ShopWebApp.dll"]