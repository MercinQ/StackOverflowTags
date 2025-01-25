# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files for dependency resolution
COPY ["StackOverflowTags/StackOverflowTags.csproj", "./StackOverflowTags/"]
COPY ["Infrastructure/Infrastructure.csproj", "./Infrastructure/"]
COPY ["Domain/Domain.csproj", "./Domain/"]
COPY ["Application/Application.csproj", "./Application/"]

# Restore dependencies
RUN dotnet restore "./StackOverflowTags/StackOverflowTags.csproj"

# Install dotnet-ef tool and add it to PATH
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR /src/StackOverflowTags
RUN dotnet build "./StackOverflowTags.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StackOverflowTags.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Start the application
ENTRYPOINT ["dotnet", "StackOverflowTags.dll"]