# Step 1: Use the official .NET SDK Windows image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-ltsc2022 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Publish the application to the out directory
RUN dotnet publish -c Release -o out

# Step 2: Use the Windows runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-ltsc2022
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/out .

# Expose the ports the app will run on
EXPOSE 7118
EXPOSE 5262

# Set the entry point to run the app
ENTRYPOINT ["ToDo_lists.exe"]

# Set environment variables
ENV Logging__LogLevel__Default="Information"
ENV Logging__LogLevel__Microsoft_AspNetCore="Warning"
ENV AllowedHosts="*"
ENV ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=ToDoLists;Username=richard.macus;Password=abc123"
ENV Authentication__SecretKey="ExtremelyVeryStrongSecretKeyForJWT"
ENV Authentication__Issuer="https://localhost:7118"
ENV Authentication__Audience="ToDoListsAPI"
ENV ASPNETCORE_URLS="https://+:7118;http://+:5262"
