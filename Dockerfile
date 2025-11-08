# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only production project files (exclude tests)
COPY ["src/TodoApi/TodoApi.csproj", "src/TodoApi/"]
COPY ["src/TodoApi.Core/TodoApi.Core.csproj", "src/TodoApi.Core/"]
COPY ["src/TodoApi.Infrastructure/TodoApi.Infrastructure.csproj", "src/TodoApi.Infrastructure/"]

# Restore only production dependencies
RUN dotnet restore "src/TodoApi/TodoApi.csproj"

# Copy only production source code
COPY ["src/", "src/"]

# Build the project
WORKDIR "/src/src/TodoApi"
RUN dotnet build "TodoApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TodoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApi.dll"]