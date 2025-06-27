# Etap bazowy
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 443

# Dodaj certyfikaty SSL
ENV ASPNETCORE_URLS="https://+:443"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/dev.crt
ENV ASPNETCORE_Kestrel__Certificates__Default__KeyPath=/https/dev.key
COPY docker/nginx/certs/dev.crt /https/dev.crt
COPY docker/nginx/certs/dev.key /https/dev.key

# Etap build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY SANSylabusApi/ ./SANSylabusApi/
COPY ServiceDefaults1/ ./ServiceDefaults1/

WORKDIR /src/SANSylabusApi
RUN dotnet restore
RUN dotnet publish "SANSylabusApi.csproj" -c Release -o /app/publish

# Etap końcowy
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SANSylabusApi.dll"]
