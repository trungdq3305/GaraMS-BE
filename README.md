Cấu Hình Backend

BE Deploy Azure
https://garamsapi.lemoncliff-682dfe26.southeastasia.azurecontainerapps.io/swagger/index.html

Dockerfile (src/GaraMS.API/Dockerfile)
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GaraMS.API/GaraMS.API.csproj", "GaraMS.API/"]
COPY ["GaraMS.Service/GaraMS.Service.csproj", "GaraMS.Service/"]
COPY ["GaraMS.Data/GaraMS.Data.csproj", "GaraMS.Data/"]
RUN dotnet restore "./GaraMS.API/GaraMS.API.csproj"
COPY . .
WORKDIR "/src/GaraMS.API"
RUN dotnet build "./GaraMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GaraMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GaraMS.API.dll"]



Docker Compose (docker-compose.yml)
services:
  garams.api:
    image: ${DOCKER_REGISTRY-}garamsapi
    build:
      context: .
      dockerfile: GaraMS.API/Dockerfile
        



CI/CD Pipeline (GitHub Actions)
File .github/workflows/publish-workflow.yml
name: Delpoy to Azure Container Apps

on:
  push:
    branches: [ master ]
    
env:
  AZURE_CONTAINER_REGISTRY: garamsapi
  CONTAINER_APP_NAME: garamsapi
  RESOURCE_GROUP: group01
jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4
    
    - name: Debug workspace
      run: ls -R

    - name: Set up .NET Core
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: "8.0.x"

    - name: Build
      run: dotnet build src/GaraMS.sln --configuration Release

    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3

    - name: Log in to ACR
      uses: docker/login-action@v3
      with:
        registry: ${{ env.AZURE_CONTAINER_REGISTRY }}.azurecr.io
        username: ${{ secrets.AZURE_REGISTRY_USERNAME }}
        password: ${{ secrets.AZURE_REGISTRY_PASSWORD }}

    - name: Build and push container image to ACR
      uses: docker/build-push-action@v6
      with:
        push: true
        tags: ${{ env.AZURE_CONTAINER_REGISTRY }}.azurecr.io/${{ env.CONTAINER_APP_NAME }}:${{ github.sha }}
        file: src/GaraMS.API/Dockerfile
        context: src

    - name: Azure Login
      uses: azure/login@v2
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}

    - name: Deploy to Azure Container Apps
      uses: azure/container-apps-deploy-action@v1
      with:
        imageToDeploy: ${{ env.AZURE_CONTAINER_REGISTRY }}.azurecr.io/${{ env.CONTAINER_APP_NAME }}:${{ github.sha }}
        resourceGroup: ${{ env.RESOURCE_GROUP }}
        containerAppName: ${{ env.CONTAINER_APP_NAME }}
        environmentVariables:
          ASPNETCORE_ENVIRONMENT=Development




Cấu hình CORS trong Backend(Program.cs)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .WithOrigins("https://gara-ms-fe-three.vercel.app")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

app.UseCors("AllowAll");


Luồng CI/CD
Git Push → GitHub Actions → 
Build & Push Docker Image → 
Deploy Azure Container Apps → 
Frontend gọi API qua domain Azure.
Entity Framework - Scaffold DB
Cài Package

dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design

Tạo Model từ DB

dotnet ef dbcontext scaffold \
"Data Source=garams.database.windows.net;Initial Catalog=GaraManagementSystem;User ID=sa1;Password=TTt192004;Trust Server Certificate=True" \
Microsoft.EntityFrameworkCore.SqlServer \
-o Models -c GaraMSDbContext --use-database-names

Database deploy on Azure



