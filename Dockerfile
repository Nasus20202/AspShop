FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build

WORKDIR /build

COPY . .

RUN dotnet restore

RUN dotnet publish ShopWebApp.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim

WORKDIR /app

COPY --from=build /build/out .

CMD ["dotnet", "ShopWebApp.dll"]