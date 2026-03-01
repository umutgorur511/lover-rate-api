# Base image: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GirlfriendRateApi.csproj", "./"]
RUN dotnet restore "./GirlfriendRateApi.csproj"
COPY . .
RUN dotnet publish "GirlfriendRateApi.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GirlfriendRateApi.dll"]