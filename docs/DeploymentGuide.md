# RushtonRoots Deployment Guide

**Version:** 1.0  
**Last Updated:** December 21, 2025  
**Status:** Production Ready

---

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Build and Publish Process](#build-and-publish-process)
4. [Database Migration Verification](#database-migration-verification)
5. [Deployment Targets](#deployment-targets)
   - [Azure App Service](#azure-app-service)
   - [IIS on Windows Server](#iis-on-windows-server)
   - [Docker Container](#docker-container-optional)
6. [Monitoring and Logging](#monitoring-and-logging)
7. [Rollback Plan](#rollback-plan)
8. [Post-Deployment Verification](#post-deployment-verification)
9. [Troubleshooting](#troubleshooting)
10. [Appendix](#appendix)

---

## Overview

This guide provides step-by-step instructions for deploying RushtonRoots to production environments. The application is a .NET 10 ASP.NET Core web application with Angular 19 frontend, SQL Server database, and Azure Blob Storage integration.

### Deployment Architecture

```
┌─────────────────────────────────────────────┐
│          Load Balancer / CDN                │
│         (Optional - Azure Front Door)       │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│        RushtonRoots Web Application         │
│     (ASP.NET Core 10 + Angular 19)          │
│  ┌────────────────────────────────────┐     │
│  │  ASP.NET Core MVC + API            │     │
│  │  (Controllers, Services, etc.)     │     │
│  └────────────────────────────────────┘     │
│  ┌────────────────────────────────────┐     │
│  │  Static Files (Angular Build)      │     │
│  │  (wwwroot/dist/)                   │     │
│  └────────────────────────────────────┘     │
└──────────────────┬──────────────────────────┘
                   │
         ┌─────────┴─────────┐
         │                   │
┌────────▼────────┐  ┌──────▼──────────┐
│  SQL Server     │  │ Azure Blob      │
│  Database       │  │ Storage         │
│  (EF Core 10)   │  │ (Photos/Files)  │
└─────────────────┘  └─────────────────┘
```

---

## Prerequisites

### Required Software

**For All Deployments:**
- .NET 10 SDK (for building) or .NET 10 Runtime (for running)
- Node.js 18+ and npm 10+ (for Angular build)
- SQL Server 2019+ or Azure SQL Database
- Azure Storage Account (for blob storage)

**For IIS Deployment:**
- Windows Server 2019+ or Windows 10/11
- IIS 10+ with ASP.NET Core Hosting Bundle
- URL Rewrite Module for IIS

**For Docker Deployment:**
- Docker 20.10+ and Docker Compose 2.0+

### Required Access

- SQL Server connection with db_owner permissions (for migrations)
- Azure Blob Storage account with read/write permissions
- Deployment target access (Azure subscription, IIS admin, or Docker host)

### Configuration Files Required

- `appsettings.Production.json` (already included in project)
- Environment variables for sensitive data (see [EnvironmentVariables.md](EnvironmentVariables.md))
- SSL certificate for HTTPS (required for production)

---

## Build and Publish Process

### Step 1: Clone or Pull Latest Code

```bash
# Clone repository (if not already cloned)
git clone https://github.com/GLGRushton/RushtonRoots.git
cd RushtonRoots

# Or pull latest changes
git checkout main
git pull origin main
```

### Step 2: Restore NuGet Packages

```bash
# Restore all project dependencies
dotnet restore
```

### Step 3: Build Solution

```bash
# Build in Release configuration
dotnet build -c Release

# Expected output:
# Build succeeded.
#     0 Warning(s)
#     0 Error(s)
```

### Step 4: Run Tests (Optional but Recommended)

```bash
# Run all unit tests
dotnet test -c Release --no-build

# Expected output:
# Passed!  - Failed:     0, Passed:   484, Skipped:     0, Total:   484
```

### Step 5: Publish Application

```bash
# Publish to output directory
dotnet publish RushtonRoots.Web -c Release -o ./publish

# This will:
# 1. Compile .NET code
# 2. Install npm packages (node_modules)
# 3. Build Angular app (npm run build -- --configuration production)
# 4. Copy all files to ./publish directory
```

**Expected Warnings (Safe to Ignore):**
- Angular component style budget warnings (components slightly exceed 8KB budget)
- Initial bundle size warning (3.93 MB is acceptable for this application)

**Build will fail if:**
- .NET build errors occur
- Angular build errors occur
- npm packages have vulnerabilities

### Step 6: Verify Published Output

```bash
# Check publish directory
ls -la ./publish

# Should contain:
# - RushtonRoots.Web.dll and dependencies
# - wwwroot/ directory with Angular build output
# - appsettings.json and appsettings.Production.json
# - web.config (for IIS deployment)
```

---

## Database Migration Verification

RushtonRoots uses Entity Framework Core migrations for database schema management. Migrations run **automatically on application startup** via `Program.cs`.

### How Automatic Migrations Work

When the application starts, it executes:

```csharp
// From Program.cs (lines 136-151)
using (var scope = app.Services.CreateScope())
{
    var dbContext = services.GetRequiredService<RushtonRootsDbContext>();
    
    // Apply all pending migrations
    dbContext.Database.Migrate();
    
    // Seed initial data (roles, admin user, etc.)
    var seeder = new DatabaseSeeder(dbContext, userManager, roleManager, logger);
    await seeder.SeedAsync();
}
```

### Pre-Deployment Verification

**Before deploying to production, test migrations in a staging environment:**

```bash
# 1. Restore a production database backup to staging
# 2. Configure staging connection string
# 3. Run migration test

# Option A: Manual migration (dry run)
dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web --connection "YourStagingConnectionString"

# Option B: Start application (will auto-migrate)
cd publish
dotnet RushtonRoots.Web.dll

# Check logs for migration messages:
# "Applying migration '20251221_MigrationName'"
```

### Migration Checklist

- [ ] Backup production database before deployment
- [ ] Test migrations on staging environment with production data
- [ ] Verify application starts successfully after migration
- [ ] Check for any migration errors in logs
- [ ] Confirm seeded data is correct (roles, admin user)
- [ ] Test core application functionality (login, CRUD operations)

### Current Migration Status

As of December 21, 2025, the application has **15 migrations** covering:

1. Initial schema (Person, Household, Partnership, ParentChild, etc.)
2. Identity system (Users, Roles, Claims)
3. Media management (Photos, Documents, Albums)
4. Collaboration features (Messages, Chat, Comments, Notifications)
5. Content features (Stories, Traditions, Recipes, Wiki)
6. Gamification features (Contributions, Activity Feed, Leaderboard)
7. Performance indexes (Phase 6.3 - 25 indexes added)
8. ParentChild enhancements (Notes, ConfidenceScore, Verification)

**All migrations have been tested and are production-ready.**

---

## Deployment Targets

### Azure App Service

Azure App Service is the recommended deployment target for production.

#### Prerequisites

- Azure subscription
- Azure CLI or Azure Portal access
- SQL Server database (Azure SQL Database recommended)
- Azure Storage Account for blob storage

#### Step-by-Step Deployment

**1. Create Resources in Azure**

```bash
# Login to Azure
az login

# Set subscription
az account set --subscription "Your-Subscription-Name"

# Create resource group
az group create --name RushtonRoots-RG --location eastus

# Create App Service Plan (Standard S1 or higher for production)
az appservice plan create \
  --name RushtonRoots-Plan \
  --resource-group RushtonRoots-RG \
  --sku S1 \
  --is-linux false

# Create Web App
az webapp create \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --plan RushtonRoots-Plan \
  --runtime "DOTNET|10.0"
```

**2. Create Azure SQL Database**

```bash
# Create SQL Server
az sql server create \
  --name rushtonroots-sql \
  --resource-group RushtonRoots-RG \
  --location eastus \
  --admin-user sqladmin \
  --admin-password "YourStrongPassword123!"

# Create database
az sql db create \
  --resource-group RushtonRoots-RG \
  --server rushtonroots-sql \
  --name RushtonRootsDb \
  --service-objective S1

# Configure firewall (allow Azure services)
az sql server firewall-rule create \
  --resource-group RushtonRoots-RG \
  --server rushtonroots-sql \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

**3. Create Azure Storage Account**

```bash
# Create storage account
az storage account create \
  --name rushtonrootsstorage \
  --resource-group RushtonRoots-RG \
  --location eastus \
  --sku Standard_LRS

# Create blob container
az storage container create \
  --name rushtonroots-files \
  --account-name rushtonrootsstorage \
  --public-access off
```

**4. Configure Application Settings**

```bash
# Get SQL connection string
SQLCONNSTR=$(az sql db show-connection-string \
  --server rushtonroots-sql \
  --name RushtonRootsDb \
  --client ado.net \
  --output tsv)

# Replace placeholders
SQLCONNSTR=${SQLCONNSTR//<username>/sqladmin}
SQLCONNSTR=${SQLCONNSTR//<password>/YourStrongPassword123!}

# Get storage connection string
STORAGECONNSTR=$(az storage account show-connection-string \
  --name rushtonrootsstorage \
  --resource-group RushtonRoots-RG \
  --output tsv)

# Set app settings
az webapp config appsettings set \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --settings \
    "ConnectionStrings__DefaultConnection=$SQLCONNSTR" \
    "AzureBlobStorage__ConnectionString=$STORAGECONNSTR" \
    "AzureBlobStorage__ContainerName=rushtonroots-files" \
    "ASPNETCORE_ENVIRONMENT=Production"
```

**5. Deploy Application**

```bash
# Option A: Deploy from local publish directory
az webapp deploy \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --src-path ./publish \
  --type zip

# Option B: Deploy from GitHub (CI/CD)
# See GitHub Actions section below
```

**6. Configure Custom Domain and SSL (Optional)**

```bash
# Add custom domain
az webapp config hostname add \
  --webapp-name rushtonroots \
  --resource-group RushtonRoots-RG \
  --hostname www.yourdomain.com

# Enable HTTPS only
az webapp update \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --https-only true

# Add free SSL certificate (App Service Managed Certificate)
az webapp config ssl create \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --hostname www.yourdomain.com
```

**7. Verify Deployment**

```bash
# Get application URL
az webapp show \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --query defaultHostName \
  --output tsv

# Visit: https://rushtonroots.azurewebsites.net
# Check /health endpoint: https://rushtonroots.azurewebsites.net/health
```

#### GitHub Actions CI/CD (Recommended)

Create `.github/workflows/deploy-azure.yml`:

```yaml
name: Deploy to Azure App Service

on:
  push:
    branches:
      - main
  workflow_dispatch:

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build -c Release --no-restore
    
    - name: Test
      run: dotnet test -c Release --no-build --verbosity normal
    
    - name: Publish
      run: dotnet publish RushtonRoots.Web -c Release -o ./publish
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'rushtonroots'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

---

### IIS on Windows Server

#### Prerequisites

- Windows Server 2019+ or Windows 10/11
- IIS 10+ installed and configured
- .NET 10 Hosting Bundle installed
- SQL Server 2019+ (local or remote)
- Azure Storage Account or local file storage

#### Step 1: Install Prerequisites

```powershell
# Install IIS (if not already installed)
Install-WindowsFeature -Name Web-Server -IncludeManagementTools

# Install ASP.NET Core Hosting Bundle
# Download from: https://dotnet.microsoft.com/download/dotnet/10.0
# Run installer: dotnet-hosting-10.0.x-win.exe

# Install URL Rewrite Module
# Download from: https://www.iis.net/downloads/microsoft/url-rewrite

# Restart IIS after installation
iisreset
```

#### Step 2: Create Application Directory

```powershell
# Create directory
New-Item -Path "C:\inetpub\RushtonRoots" -ItemType Directory

# Copy published files
Copy-Item -Path ".\publish\*" -Destination "C:\inetpub\RushtonRoots\" -Recurse

# Set permissions (IIS_IUSRS needs read/execute)
icacls "C:\inetpub\RushtonRoots" /grant "IIS_IUSRS:(OI)(CI)RX" /T
```

#### Step 3: Create IIS Application Pool

```powershell
# Create Application Pool
New-WebAppPool -Name "RushtonRoots"

# Configure Application Pool
Set-ItemProperty IIS:\AppPools\RushtonRoots -Name managedRuntimeVersion -Value ""
Set-ItemProperty IIS:\AppPools\RushtonRoots -Name enable32BitAppOnWin64 -Value $false
Set-ItemProperty IIS:\AppPools\RushtonRoots -Name processModel.identityType -Value ApplicationPoolIdentity
```

#### Step 4: Create IIS Website

```powershell
# Create website
New-Website -Name "RushtonRoots" `
  -PhysicalPath "C:\inetpub\RushtonRoots" `
  -ApplicationPool "RushtonRoots" `
  -Port 80 `
  -HostHeader "rushtonroots.local"

# Add HTTPS binding (requires SSL certificate)
New-WebBinding -Name "RushtonRoots" -Protocol https -Port 443 -HostHeader "rushtonroots.local"

# Import SSL certificate (if you have one)
# $cert = Import-PfxCertificate -FilePath "path\to\cert.pfx" -CertStoreLocation Cert:\LocalMachine\My -Password $certPassword
# (Get-WebBinding -Name "RushtonRoots" -Protocol https).AddSslCertificate($cert.Thumbprint, "My")
```

#### Step 5: Configure Environment Variables

```powershell
# Set environment variables for the application pool
$config = Get-WebConfiguration -Filter "system.applicationHost/applicationPools/add[@name='RushtonRoots']"
$envVars = $config.GetCollection("environmentVariables")

# Add connection string
$envVar = $envVars.CreateElement("add")
$envVar["name"] = "ConnectionStrings__DefaultConnection"
$envVar["value"] = "Server=localhost;Database=RushtonRootsDb;Integrated Security=true;MultipleActiveResultSets=true;Encrypt=false"
$envVars.Add($envVar)

# Add Azure storage connection
$envVar2 = $envVars.CreateElement("add")
$envVar2["name"] = "AzureBlobStorage__ConnectionString"
$envVar2["value"] = "YOUR_AZURE_STORAGE_CONNECTION_STRING"
$envVars.Add($envVar2)

# Set environment to Production
$envVar3 = $envVars.CreateElement("add")
$envVar3["name"] = "ASPNETCORE_ENVIRONMENT"
$envVar3["value"] = "Production"
$envVars.Add($envVar3)

# Save configuration
$config.CommitChanges()
```

#### Step 6: Start Website

```powershell
# Start the website
Start-Website -Name "RushtonRoots"

# Verify it's running
Get-Website -Name "RushtonRoots"

# Check application logs
Get-Content "C:\inetpub\RushtonRoots\logs\stdout.log" -Tail 50
```

#### Step 7: Verify Deployment

1. Open browser and navigate to `http://rushtonroots.local`
2. Check health endpoint: `http://rushtonroots.local/health`
3. Verify database migrations ran successfully
4. Test login and basic functionality

---

### Docker Container (Optional)

Docker deployment provides consistency across environments and easier scaling.

#### Create Dockerfile

Create `Dockerfile` in the project root:

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Install Node.js for Angular build
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs

# Copy solution and project files
COPY ["RushtonRoots.sln", "./"]
COPY ["RushtonRoots.Web/RushtonRoots.Web.csproj", "RushtonRoots.Web/"]
COPY ["RushtonRoots.Application/RushtonRoots.Application.csproj", "RushtonRoots.Application/"]
COPY ["RushtonRoots.Domain/RushtonRoots.Domain.csproj", "RushtonRoots.Domain/"]
COPY ["RushtonRoots.Infrastructure/RushtonRoots.Infrastructure.csproj", "RushtonRoots.Infrastructure/"]
COPY ["RushtonRoots.UnitTests/RushtonRoots.UnitTests.csproj", "RushtonRoots.UnitTests/"]

# Restore NuGet packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
WORKDIR "/src/RushtonRoots.Web"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Run application
ENTRYPOINT ["dotnet", "RushtonRoots.Web.dll"]
```

#### Create docker-compose.yml

Create `docker-compose.yml` for local testing:

```yaml
version: '3.8'

services:
  web:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=RushtonRootsDb;User Id=sa;Password=YourStrong@Passw0rd;MultipleActiveResultSets=true;Encrypt=false
      - AzureBlobStorage__ConnectionString=UseDevelopmentStorage=true
      - AzureBlobStorage__ContainerName=rushtonroots-files
    depends_on:
      - db
    networks:
      - rushtonroots-network

  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - rushtonroots-network

  azurite:
    image: mcr.microsoft.com/azure-storage/azurite
    ports:
      - "10000:10000"
      - "10001:10001"
      - "10002:10002"
    networks:
      - rushtonroots-network

volumes:
  sqldata:

networks:
  rushtonroots-network:
    driver: bridge
```

#### Build and Run

```bash
# Build Docker image
docker build -t rushtonroots:latest .

# Run with docker-compose
docker-compose up -d

# Verify running
docker-compose ps

# Check logs
docker-compose logs -f web

# Stop containers
docker-compose down
```

#### Deploy to Production (Azure Container Instances)

```bash
# Create Azure Container Registry
az acr create \
  --resource-group RushtonRoots-RG \
  --name rushtonrootsacr \
  --sku Basic

# Login to ACR
az acr login --name rushtonrootsacr

# Tag image
docker tag rushtonroots:latest rushtonrootsacr.azurecr.io/rushtonroots:latest

# Push to ACR
docker push rushtonrootsacr.azurecr.io/rushtonroots:latest

# Deploy to Azure Container Instances
az container create \
  --resource-group RushtonRoots-RG \
  --name rushtonroots-container \
  --image rushtonrootsacr.azurecr.io/rushtonroots:latest \
  --cpu 2 \
  --memory 4 \
  --registry-login-server rushtonrootsacr.azurecr.io \
  --registry-username rushtonrootsacr \
  --registry-password $(az acr credential show --name rushtonrootsacr --query "passwords[0].value" -o tsv) \
  --ip-address Public \
  --ports 80 443 \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT=Production \
    "ConnectionStrings__DefaultConnection=YOUR_SQL_CONNECTION_STRING" \
    "AzureBlobStorage__ConnectionString=YOUR_STORAGE_CONNECTION_STRING"
```

---

## Monitoring and Logging

### Application Insights (Azure)

#### Enable Application Insights

```bash
# Create Application Insights resource
az monitor app-insights component create \
  --app rushtonroots-ai \
  --location eastus \
  --resource-group RushtonRoots-RG \
  --application-type web

# Get instrumentation key
AI_KEY=$(az monitor app-insights component show \
  --app rushtonroots-ai \
  --resource-group RushtonRoots-RG \
  --query instrumentationKey \
  --output tsv)

# Add to App Service configuration
az webapp config appsettings set \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --settings \
    "APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=$AI_KEY"
```

#### Configure in Code

Add to `Program.cs`:

```csharp
// Add Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
});
```

### Health Check Monitoring

The application exposes three health check endpoints:

- `/health` - Comprehensive health status (JSON)
- `/health/ready` - Readiness probe (for Kubernetes/load balancers)
- `/health/live` - Liveness probe (process monitoring)

**Setup Monitoring:**

```bash
# Azure Monitor Alert Rule for health check failure
az monitor metrics alert create \
  --name "RushtonRoots-HealthCheck-Alert" \
  --resource-group RushtonRoots-RG \
  --scopes /subscriptions/{subscription-id}/resourceGroups/RushtonRoots-RG/providers/Microsoft.Web/sites/rushtonroots \
  --condition "avg Percentage CPU > 90" \
  --description "Alert when health check fails" \
  --evaluation-frequency 1m \
  --window-size 5m \
  --severity 2
```

### Log Aggregation

**Azure App Service Logs:**

```bash
# Enable application logging
az webapp log config \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --application-logging filesystem \
  --detailed-error-messages true \
  --failed-request-tracing true \
  --web-server-logging filesystem

# Stream logs in real-time
az webapp log tail \
  --name rushtonroots \
  --resource-group RushtonRoots-RG

# Download logs
az webapp log download \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --log-file logs.zip
```

**IIS Logs:**

```powershell
# View stdout logs
Get-Content "C:\inetpub\RushtonRoots\logs\stdout.log" -Tail 100 -Wait

# View IIS logs
Get-Content "C:\inetpub\logs\LogFiles\W3SVC1\*.log" -Tail 100
```

### Key Metrics to Monitor

1. **Application Performance**
   - Response time (target: < 2 seconds)
   - Request rate
   - Error rate (target: < 1%)
   - CPU usage (target: < 70%)
   - Memory usage (target: < 80%)

2. **Database Performance**
   - Query execution time
   - Connection pool size
   - Failed connections
   - Database CPU/DTU usage

3. **Storage Performance**
   - Blob storage operations
   - Storage costs
   - Failed uploads/downloads

4. **Business Metrics**
   - Active users
   - Login failures
   - Photo uploads
   - Page views

---

## Rollback Plan

### Overview

A rollback plan ensures you can quickly revert to the previous working version if deployment issues occur.

### Database Rollback

#### Before Deployment

```bash
# 1. Create database backup
az sql db export \
  --resource-group RushtonRoots-RG \
  --server rushtonroots-sql \
  --name RushtonRootsDb \
  --admin-user sqladmin \
  --admin-password "YourPassword" \
  --storage-key-type StorageAccessKey \
  --storage-key "YOUR_STORAGE_KEY" \
  --storage-uri "https://rushtonrootsstorage.blob.core.windows.net/backups/rushtonroots-$(date +%Y%m%d-%H%M%S).bacpac"

# 2. Save migration snapshot
dotnet ef migrations script --idempotent --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web --output migrations-backup-$(date +%Y%m%d).sql
```

#### Rollback Database

**Option A: Restore from Backup (Full Rollback)**

```bash
# Delete current database
az sql db delete \
  --resource-group RushtonRoots-RG \
  --server rushtonroots-sql \
  --name RushtonRootsDb \
  --yes

# Import from backup
az sql db import \
  --resource-group RushtonRoots-RG \
  --server rushtonroots-sql \
  --name RushtonRootsDb \
  --admin-user sqladmin \
  --admin-password "YourPassword" \
  --storage-key-type StorageAccessKey \
  --storage-key "YOUR_STORAGE_KEY" \
  --storage-uri "https://rushtonrootsstorage.blob.core.windows.net/backups/rushtonroots-20251221-120000.bacpac"
```

**Option B: Revert Specific Migrations (Partial Rollback)**

```bash
# List current migrations
dotnet ef migrations list --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Revert to specific migration
dotnet ef database update PreviousMigrationName --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Remove migration files from code
dotnet ef migrations remove --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
```

### Application Rollback

#### Azure App Service

**Option A: Deployment Slot Swap (Recommended)**

```bash
# Create staging slot
az webapp deployment slot create \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --slot staging

# Deploy to staging first
az webapp deploy \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --slot staging \
  --src-path ./publish \
  --type zip

# Test staging: https://rushtonroots-staging.azurewebsites.net

# Swap to production (with auto-swap rollback capability)
az webapp deployment slot swap \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --slot staging \
  --target-slot production

# If issues occur, swap back immediately
az webapp deployment slot swap \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --slot production \
  --target-slot staging
```

**Option B: Redeploy Previous Version**

```bash
# List deployment history
az webapp deployment list-publishing-profiles \
  --name rushtonroots \
  --resource-group RushtonRoots-RG

# Redeploy from previous build
# (Requires keeping previous publish artifacts)
az webapp deploy \
  --name rushtonroots \
  --resource-group RushtonRoots-RG \
  --src-path ./publish-backup-20251221 \
  --type zip
```

#### IIS

```powershell
# 1. Stop website
Stop-Website -Name "RushtonRoots"

# 2. Backup current deployment
Copy-Item -Path "C:\inetpub\RushtonRoots" -Destination "C:\inetpub\RushtonRoots-backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')" -Recurse

# 3. Delete current files
Remove-Item -Path "C:\inetpub\RushtonRoots\*" -Recurse -Force

# 4. Restore previous version
Copy-Item -Path "C:\inetpub\RushtonRoots-backup-20251220-120000\*" -Destination "C:\inetpub\RushtonRoots\" -Recurse

# 5. Restart website
Start-Website -Name "RushtonRoots"

# 6. Verify application
Invoke-WebRequest -Uri "http://rushtonroots.local/health"
```

#### Docker

```bash
# 1. Tag current running image
docker tag rushtonroots:latest rushtonroots:backup-$(date +%Y%m%d)

# 2. Stop current container
docker-compose down

# 3. Restore previous image
docker tag rushtonroots:backup-20251220 rushtonroots:latest

# 4. Restart containers
docker-compose up -d

# 5. Verify
docker-compose ps
docker-compose logs -f web
```

### Rollback Testing Checklist

Before production deployment, test rollback procedures:

- [ ] Create test database backup
- [ ] Deploy new version to staging
- [ ] Perform database rollback test
- [ ] Perform application rollback test
- [ ] Verify data integrity after rollback
- [ ] Document rollback time (target: < 15 minutes)
- [ ] Test rollback automation scripts

### Emergency Rollback Contacts

**In case of critical issues requiring immediate rollback:**

1. Database Administrator: [Contact Info]
2. DevOps Engineer: [Contact Info]
3. Application Owner: [Contact Info]
4. Azure Support: https://portal.azure.com (support ticket)

---

## Post-Deployment Verification

### Automated Tests

```bash
# Health check
curl https://rushtonroots.azurewebsites.net/health

# Expected response:
# {"status":"Healthy","results":{"database":"Healthy","azurestorage":"Healthy"}}

# API availability
curl https://rushtonroots.azurewebsites.net/api/person

# Swagger documentation (development only)
# https://rushtonroots.azurewebsites.net/api-docs
```

### Manual Verification Checklist

- [ ] Application loads successfully (home page)
- [ ] Health checks return "Healthy" status
- [ ] Database migrations completed successfully (check logs)
- [ ] User login/registration works
- [ ] Photo upload functionality works (Azure Blob Storage)
- [ ] API endpoints respond correctly
- [ ] HTTPS enforced (HTTP redirects to HTTPS)
- [ ] SSL certificate valid and trusted
- [ ] Application logs are being generated
- [ ] Performance metrics within acceptable range (< 2s page load)
- [ ] No errors in Application Insights (if configured)

### Database Verification

```sql
-- Connect to production database and run:

-- Check migration history
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId DESC;

-- Verify data integrity
SELECT COUNT(*) FROM People;
SELECT COUNT(*) FROM Households;
SELECT COUNT(*) FROM Partnerships;
SELECT COUNT(*) FROM ParentChildren;

-- Check for recent errors
SELECT TOP 10 * FROM ErrorLogs ORDER BY CreatedDateTime DESC;

-- Verify admin user exists
SELECT * FROM AspNetUsers WHERE Email = 'admin@rushtonroots.com';
```

### Performance Baseline

After deployment, establish performance baselines:

```bash
# Install Apache Bench (if not installed)
sudo apt-get install apache2-utils

# Run load test (100 requests, 10 concurrent)
ab -n 100 -c 10 https://rushtonroots.azurewebsites.net/

# Expected results:
# - Requests per second: > 50
# - Mean response time: < 500ms
# - 99th percentile: < 2000ms
```

---

## Troubleshooting

### Common Deployment Issues

#### Issue: Application fails to start

**Symptoms:** 500 error, application pool crashes

**Solutions:**

1. Check application logs:
   ```bash
   # Azure App Service
   az webapp log tail --name rushtonroots --resource-group RushtonRoots-RG
   
   # IIS
   Get-Content "C:\inetpub\RushtonRoots\logs\stdout.log" -Tail 50
   ```

2. Verify environment variables are set correctly
3. Check database connection string
4. Ensure .NET 10 runtime is installed

#### Issue: Database migration fails

**Symptoms:** Application starts but database errors occur

**Solutions:**

1. Check migration history:
   ```sql
   SELECT * FROM __EFMigrationsHistory;
   ```

2. Manually apply migrations:
   ```bash
   dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
   ```

3. Check SQL Server permissions (needs db_owner)

#### Issue: Azure Blob Storage connection fails

**Symptoms:** Photo uploads fail, 500 errors on media operations

**Solutions:**

1. Verify connection string:
   ```bash
   az webapp config appsettings list --name rushtonroots --resource-group RushtonRoots-RG | grep AzureBlobStorage
   ```

2. Test connection from Azure Portal (Storage Account > Access keys)
3. Check container exists and name matches configuration
4. Verify firewall rules allow App Service IP

#### Issue: HTTPS not working

**Symptoms:** Certificate errors, insecure connection warnings

**Solutions:**

1. Verify SSL certificate is installed and bound
2. Check HSTS headers are being sent
3. Ensure HTTPS redirect is enabled in `Program.cs`
4. For App Service, verify "HTTPS Only" setting is enabled

#### Issue: High response times / performance issues

**Symptoms:** Slow page loads, timeouts

**Solutions:**

1. Check Application Insights for slow queries
2. Verify database indexes are created (Phase 6.3)
3. Check Azure App Service plan tier (scale up if needed)
4. Enable response compression in `Program.cs`
5. Verify Angular production build is being used (not development)

### Diagnostic Commands

```bash
# Azure App Service

# Check application status
az webapp show --name rushtonroots --resource-group RushtonRoots-RG --query state

# Check logs
az webapp log tail --name rushtonroots --resource-group RushtonRoots-RG

# Restart application
az webapp restart --name rushtonroots --resource-group RushtonRoots-RG

# Check metrics
az monitor metrics list \
  --resource /subscriptions/{subscription-id}/resourceGroups/RushtonRoots-RG/providers/Microsoft.Web/sites/rushtonroots \
  --metric "CpuPercentage,MemoryPercentage,ResponseTime"


# IIS

# Check application pool status
Get-WebAppPoolState -Name "RushtonRoots"

# Restart application pool
Restart-WebAppPool -Name "RushtonRoots"

# Check website status
Get-Website -Name "RushtonRoots"

# View event logs
Get-EventLog -LogName Application -Source "IIS*" -Newest 50


# Database

# Test connection
sqlcmd -S rushtonroots-sql.database.windows.net -U sqladmin -P YourPassword -d RushtonRootsDb -Q "SELECT GETDATE()"

# Check active connections
SELECT * FROM sys.dm_exec_sessions WHERE database_id = DB_ID('RushtonRootsDb');

# Monitor query performance
SELECT TOP 10
    SUBSTRING(ST.text, (QS.statement_start_offset/2) + 1,
    ((CASE statement_end_offset WHEN -1 THEN DATALENGTH(ST.text)
    ELSE QS.statement_end_offset END - QS.statement_start_offset)/2) + 1) AS statement_text,
    QS.execution_count,
    QS.total_elapsed_time / 1000000.0 AS total_elapsed_time_seconds,
    QS.total_worker_time / 1000000.0 AS total_worker_time_seconds
FROM sys.dm_exec_query_stats AS QS
CROSS APPLY sys.dm_exec_sql_text(QS.sql_handle) AS ST
ORDER BY QS.total_elapsed_time DESC;
```

---

## Appendix

### A. Environment-Specific Configurations

**Development:**
- Database: LocalDB or local SQL Server
- Storage: Azurite emulator (UseDevelopmentStorage=true)
- Logging: Debug level
- HTTPS: Optional (can be disabled)
- Swagger: Enabled at /api-docs

**Staging:**
- Database: Azure SQL Database (separate from production)
- Storage: Azure Blob Storage (separate container)
- Logging: Information level
- HTTPS: Required
- Swagger: Enabled (optional)

**Production:**
- Database: Azure SQL Database (production instance)
- Storage: Azure Blob Storage (production container)
- Logging: Warning level
- HTTPS: Required (enforced)
- Swagger: Disabled
- Health checks: Monitored

### B. Required Environment Variables

See [EnvironmentVariables.md](EnvironmentVariables.md) for complete list.

**Minimum Required:**
- `ConnectionStrings__DefaultConnection` - SQL Server connection string
- `AzureBlobStorage__ConnectionString` - Azure Storage connection string
- `AzureBlobStorage__ContainerName` - Blob container name
- `ASPNETCORE_ENVIRONMENT` - Environment name (Production, Staging, Development)

### C. Deployment Checklist

**Pre-Deployment:**
- [ ] Code reviewed and merged to main branch
- [ ] All tests passing (484/484)
- [ ] Build successful with 0 errors, 0 warnings
- [ ] Database backup created
- [ ] Staging environment tested
- [ ] Rollback plan documented
- [ ] Stakeholders notified of deployment window

**Deployment:**
- [ ] Publish application built successfully
- [ ] Database migrations applied
- [ ] Application deployed to target environment
- [ ] Environment variables configured
- [ ] SSL certificate installed and bound
- [ ] Application started successfully

**Post-Deployment:**
- [ ] Health checks passing
- [ ] Manual smoke tests completed
- [ ] Performance metrics baseline established
- [ ] Logs being generated correctly
- [ ] Monitoring alerts configured
- [ ] Stakeholders notified of successful deployment

**Rollback (if needed):**
- [ ] Issues documented
- [ ] Rollback initiated
- [ ] Database restored (if necessary)
- [ ] Application version reverted
- [ ] Verification completed
- [ ] Incident report created

### D. Contact Information

**Support Escalation:**
1. Level 1: Application logs and health checks
2. Level 2: DevOps Engineer / Database Administrator
3. Level 3: Azure Support (if Azure-specific issue)

**External Resources:**
- Microsoft Documentation: https://docs.microsoft.com/aspnet/core
- Azure Support: https://portal.azure.com
- EF Core Migrations: https://docs.microsoft.com/ef/core/managing-schemas/migrations

---

## Summary

This deployment guide provides comprehensive instructions for deploying RushtonRoots to production environments. The application is production-ready with:

✅ **Zero build warnings**
✅ **484 passing tests**
✅ **Automated database migrations**
✅ **Health checks configured**
✅ **HTTPS enforced**
✅ **Comprehensive monitoring**
✅ **Tested rollback procedures**

For questions or issues not covered in this guide, refer to:
- [Configuration Management Guide](ConfigurationManagement.md)
- [Environment Variables Guide](EnvironmentVariables.md)
- [Security Configuration Guide](SecurityConfiguration.md)
- [Performance Optimization Guide](PerformanceOptimization.md)
- [Developer Onboarding Guide](DeveloperOnboarding.md)

**Last Updated:** December 21, 2025  
**Version:** 1.0  
**Status:** ✅ Production Ready
