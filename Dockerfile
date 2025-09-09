FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /build

COPY . .

RUN dotnet restore

RUN dotnet publish ShopWebApp.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine

RUN apk add --no-cache icu-libs tzdata

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

COPY --from=build /build/out .

CMD ["dotnet", "ShopWebApp.dll"]