# Step 1: Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project files
COPY *.csproj ./

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Publish the application to the /out directory
RUN dotnet publish -c Release -o /out

# Step 2: Use the runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /out ./

# Expose the ports the app will use
EXPOSE 7118
EXPOSE 5262

# Set the entry point to start the application
ENTRYPOINT ["dotnet", "ToDo_lists.dll"]

# Set environment variables (optional)
ENV Logging__LogLevel__Default="Information"
ENV Logging__LogLevel__Microsoft_AspNetCore="Warning"
ENV AllowedHosts="*"
#ENV ConnectionStrings__DefaultConnection="Host=database;Port=5432;Database=ToDoLists;Username=richard.macus;Password=abc123"
ENV Authentication__SecretKey="ExtremelyVeryStrongSecretKeyForJWT"
ENV Authentication__Issuer="http://localhost:7118"
ENV Authentication__Audience="ToDoListsAPI"
ENV ASPNETCORE_URLS="http://+:7118;http://+:5262"
ENV AI_MODULE_HOST = "ai_module"
ENV AI_MODULE_PORT = "8000"