FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Frontend/ ./Frontend/
WORKDIR /src/Frontend
RUN dotnet restore
RUN dotnet publish "Frontend.csproj" -c Release -o /app/publish

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot ./
COPY docker/nginx/nginx.conf /etc/nginx/conf.d/default.conf
COPY docker/nginx/certs/dev.crt /etc/nginx/certs/dev.crt
COPY docker/nginx/certs/dev.key /etc/nginx/certs/dev.key
EXPOSE 443
