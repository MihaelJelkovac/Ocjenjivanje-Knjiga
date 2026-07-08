# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# Copy project files
COPY ["ispit/Lab5.csproj", "ispit/"]

# Restore dependencies
RUN dotnet restore "ispit/Lab5.csproj"

# Copy all source code
COPY . .

# Build the application
RUN dotnet build "ispit/Lab5.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ispit/Lab5.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Create data directory for SQLite database
RUN mkdir -p /app/data

# Expose port
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the application
ENTRYPOINT ["dotnet", "Lab5.dll"]

