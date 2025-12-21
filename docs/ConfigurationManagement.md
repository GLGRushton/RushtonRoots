# RushtonRoots Configuration Management Guide

**Date:** December 2025  
**Version:** 1.0  
**Phase:** 7.1 - Production Configuration  
**Status:** Complete

---

## Table of Contents

1. [Overview](#overview)
2. [Configuration Files](#configuration-files)
3. [Environment Variables](#environment-variables)
4. [Health Checks](#health-checks)
5. [Logging Configuration](#logging-configuration)
6. [Azure Key Vault Integration](#azure-key-vault-integration)
7. [Production Deployment Checklist](#production-deployment-checklist)
8. [Troubleshooting](#troubleshooting)

---

## Overview

RushtonRoots uses ASP.NET Core's configuration system with a hierarchical approach:

1. **appsettings.json** - Base configuration (all environments)
2. **appsettings.{Environment}.json** - Environment-specific overrides
3. **Environment Variables** - Runtime configuration (secrets)
4. **Azure Key Vault** - Centralized secrets management (optional, recommended for production)

### Configuration Priority

Later sources override earlier ones:
```
appsettings.json 
  → appsettings.Production.json 
    → Environment Variables 
      → Azure Key Vault
```

---

## Configuration Files

### appsettings.json (Base Configuration)

**Location:** `RushtonRoots.Web/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.;Initial Catalog=RushtonRoots;..."
  },
  "AzureBlobStorage": {
    "ConnectionString": "",
    "ContainerName": "rushtonroots-files",
    "ThumbnailSizes": [
      { "Name": "small", "Width": 200, "Height": 200 },
      { "Name": "medium", "Width": 400, "Height": 400 }
    ],
    "ThumbnailQuality": 85
  }
}
```

**Notes:**
- Connection strings are empty by default (filled via environment variables)
- Thumbnail configuration is consistent across all environments
- Logging defaults to `Information` level

---

### appsettings.Development.json

**Location:** `RushtonRoots.Web/appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Initial Catalog=RushtonRoots_Dev;..."
  },
  "AzureBlobStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "rushtonroots-files-dev"
  }
}
```

**Features:**
- ✅ Verbose logging for debugging (`Debug` level)
- ✅ LocalDB for local development
- ✅ Azurite emulator for Azure Storage (`UseDevelopmentStorage=true`)
- ✅ Separate container name to avoid conflicts

---

### appsettings.Production.json (NEW)

**Location:** `RushtonRoots.Web/appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "System": "Error"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "[yyyy-MM-dd HH:mm:ss] ",
      "LogLevel": {
        "Default": "Warning"
      }
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "AzureBlobStorage": {
    "ConnectionString": "",
    "ContainerName": "rushtonroots-files"
  },
  "HealthChecks": {
    "Enabled": true,
    "DatabaseCheckEnabled": true,
    "AzureStorageCheckEnabled": true
  }
}
```

**Features:**
- ✅ Minimal logging (`Warning` level) for performance
- ✅ Empty connection strings (set via environment variables or Key Vault)
- ✅ Timestamps included in console logs
- ✅ Health checks enabled by default
- ✅ Separate scoping for structured logging

**Security:**
- ❌ No secrets in this file (committed to source control)
- ✅ Connection strings sourced from environment variables
- ✅ Azure Key Vault recommended for secrets management

---

## Environment Variables

See [EnvironmentVariables.md](EnvironmentVariables.md) for comprehensive documentation.

### Quick Reference

**Required in Production:**
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Server=tcp:..."
export AzureBlobStorage__ConnectionString="DefaultEndpointsProtocol=https;..."
```

**Optional:**
```bash
export AZURE_KEY_VAULT_ENDPOINT="https://rushtonroots-keyvault.vault.azure.net/"
export APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=...;"
export Logging__LogLevel__Default="Information"
```

**Naming Convention:**
- Use double underscores (`__`) for nested configuration
- Example: `ConnectionStrings__DefaultConnection` maps to `ConnectionStrings:DefaultConnection`

---

## Health Checks

### Overview

Health checks verify that the application and its dependencies are functioning correctly. Three endpoints are available:

1. **`/health`** - Comprehensive health status (all checks)
2. **`/health/ready`** - Readiness probe (critical services only)
3. **`/health/live`** - Liveness probe (app is running)

### Implementation

**Program.cs Configuration:**

```csharp
// Add Health Checks
var healthChecksBuilder = builder.Services.AddHealthChecks();

// Add database health check
healthChecksBuilder.AddDbContextCheck<RushtonRootsDbContext>(
    name: "database",
    tags: new[] { "db", "sql", "ready" });

// Add Azure Blob Storage health check
var azureStorageConnectionString = builder.Configuration["AzureBlobStorage:ConnectionString"];
if (!string.IsNullOrEmpty(azureStorageConnectionString) && 
    azureStorageConnectionString != "UseDevelopmentStorage=true")
{
    healthChecksBuilder.AddAzureBlobStorage(
        azureStorageConnectionString,
        name: "azurestorage",
        tags: new[] { "storage", "azure", "ready" });
}
```

### Health Check Endpoints

#### 1. `/health` - Full Health Check

Returns comprehensive status of all configured health checks.

**Request:**
```bash
curl https://rushtonroots.azurewebsites.net/health
```

**Response (Healthy):**
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "description": null,
      "duration": 45.2,
      "exception": null,
      "data": {}
    },
    {
      "name": "azurestorage",
      "status": "Healthy",
      "description": null,
      "duration": 120.5,
      "exception": null,
      "data": {}
    }
  ],
  "totalDuration": 165.7
}
```

**Response (Unhealthy):**
```json
{
  "status": "Unhealthy",
  "checks": [
    {
      "name": "database",
      "status": "Unhealthy",
      "description": null,
      "duration": 5002.3,
      "exception": "Connection timeout",
      "data": {}
    },
    {
      "name": "azurestorage",
      "status": "Healthy",
      "description": null,
      "duration": 95.1,
      "exception": null,
      "data": {}
    }
  ],
  "totalDuration": 5097.4
}
```

**HTTP Status Codes:**
- `200 OK` - All checks healthy
- `503 Service Unavailable` - One or more checks unhealthy

---

#### 2. `/health/ready` - Readiness Probe

Kubernetes/container orchestration readiness check. Only includes checks tagged with `"ready"`.

**Purpose:** Determines if the application is ready to receive traffic.

**Request:**
```bash
curl https://rushtonroots.azurewebsites.net/health/ready
```

**Response:**
```
Healthy
```

**HTTP Status Codes:**
- `200 OK` - Ready to serve traffic
- `503 Service Unavailable` - Not ready (database or storage unavailable)

**Use Cases:**
- Kubernetes readiness probe
- Load balancer health checks
- Zero-downtime deployments

---

#### 3. `/health/live` - Liveness Probe

Simple check to verify the application process is running.

**Purpose:** Determines if the application should be restarted.

**Request:**
```bash
curl https://rushtonroots.azurewebsites.net/health/live
```

**Response:**
```
Healthy
```

**HTTP Status Codes:**
- `200 OK` - Application is alive
- No response - Application crashed/hung (restart required)

**Use Cases:**
- Kubernetes liveness probe
- Process monitoring
- Auto-restart triggers

---

### Kubernetes Integration

**deployment.yaml Example:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: rushtonroots
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: web
        image: rushtonroots:latest
        ports:
        - containerPort: 8080
        
        # Liveness probe - restart if fails
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3
        
        # Readiness probe - remove from service if fails
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 2
```

---

### Azure App Service Integration

**Application Insights Availability Tests:**

1. Navigate to **Application Insights** → **Availability**
2. Add new test:
   - **Test type:** URL ping test
   - **URL:** `https://rushtonroots.azurewebsites.net/health`
   - **Test frequency:** 5 minutes
   - **Test locations:** Select 5+ global locations
   - **Success criteria:** HTTP 200, response time < 5s
3. Configure alerts for failures

---

### Monitoring and Alerts

**Recommended Alerts:**

1. **Health Check Failures:**
   - Trigger: `/health` returns 503 for >2 minutes
   - Action: Send alert to ops team, auto-restart if >5 minutes

2. **Database Connectivity:**
   - Trigger: Database health check fails
   - Action: Page on-call engineer, check connection string

3. **Azure Storage Issues:**
   - Trigger: Azure Storage health check fails
   - Action: Verify storage account, check connection string

4. **Response Time:**
   - Trigger: Health check duration >5 seconds
   - Action: Investigate performance degradation

---

## Logging Configuration

### Production Logging Levels

**appsettings.Production.json:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",              // General logging
      "Microsoft.AspNetCore": "Warning", // ASP.NET Core framework
      "Microsoft.EntityFrameworkCore": "Warning", // EF Core (no SQL queries)
      "System": "Error"                  // System libraries
    }
  }
}
```

**Rationale:**
- ✅ `Warning` level reduces log volume in production
- ✅ EF Core at `Warning` prevents SQL query logging (performance + security)
- ✅ System errors only logged to reduce noise
- ✅ Can override via environment variables for debugging

---

### Development Logging Levels

**appsettings.Development.json:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",                // Verbose logging
      "System": "Information",
      "Microsoft": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Features:**
- ✅ `Debug` level shows detailed application logs
- ✅ EF Core query logging enabled (in AutofacModule.cs)
- ✅ Helpful for troubleshooting during development

---

### Overriding Logging at Runtime

**Temporarily increase logging in production:**

```bash
# Azure App Service - set environment variable
az webapp config appsettings set \
  --name rushtonroots-app \
  --resource-group rushtonroots-rg \
  --settings Logging__LogLevel__Default=Information

# Restart required for environment variable changes
az webapp restart \
  --name rushtonroots-app \
  --resource-group rushtonroots-rg
```

**Docker:**
```bash
docker run -e Logging__LogLevel__Default=Information rushtonroots:latest
```

---

### Application Insights Integration (Optional)

**Add NuGet Package:**
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

**Program.cs:**
```csharp
// Add Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry();
```

**Environment Variable:**
```bash
export APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=...;IngestionEndpoint=...;"
```

**Benefits:**
- ✅ Centralized logging and telemetry
- ✅ Performance monitoring
- ✅ Dependency tracking (SQL, HTTP calls)
- ✅ Exception tracking with stack traces
- ✅ Custom metrics and events

---

## Azure Key Vault Integration

See [EnvironmentVariables.md](EnvironmentVariables.md#azure-key-vault-integration) for comprehensive setup.

### Quick Start

**1. Install NuGet Packages:**
```bash
cd RushtonRoots.Web
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

**2. Update Program.cs:**
```csharp
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault if configured
var keyVaultEndpoint = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
if (!string.IsNullOrEmpty(keyVaultEndpoint))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint),
        new DefaultAzureCredential());
}

// Rest of configuration...
```

**3. Create Key Vault and Add Secrets:**
```bash
# Create Key Vault
az keyvault create \
  --name rushtonroots-keyvault \
  --resource-group rushtonroots-rg \
  --location eastus

# Add secrets (use -- instead of :)
az keyvault secret set \
  --vault-name rushtonroots-keyvault \
  --name ConnectionStrings--DefaultConnection \
  --value "Server=tcp:rushtonroots.database.windows.net,1433;..."

az keyvault secret set \
  --vault-name rushtonroots-keyvault \
  --name AzureBlobStorage--ConnectionString \
  --value "DefaultEndpointsProtocol=https;AccountName=rushtonrootsstorage;..."
```

**4. Configure Managed Identity:**
```bash
# Enable system-assigned managed identity
az webapp identity assign \
  --name rushtonroots-app \
  --resource-group rushtonroots-rg

# Grant access to Key Vault
PRINCIPAL_ID=$(az webapp identity show \
  --name rushtonroots-app \
  --resource-group rushtonroots-rg \
  --query principalId -o tsv)

az role assignment create \
  --assignee $PRINCIPAL_ID \
  --role "Key Vault Secrets User" \
  --scope /subscriptions/{sub-id}/resourceGroups/rushtonroots-rg/providers/Microsoft.KeyVault/vaults/rushtonroots-keyvault
```

**5. Set Environment Variable:**
```bash
az webapp config appsettings set \
  --name rushtonroots-app \
  --resource-group rushtonroots-rg \
  --settings AZURE_KEY_VAULT_ENDPOINT=https://rushtonroots-keyvault.vault.azure.net/
```

---

## Production Deployment Checklist

### Pre-Deployment

- [ ] **Configuration Files Created:**
  - [ ] `appsettings.Production.json` exists
  - [ ] No secrets in configuration files (verified)
  - [ ] `.gitignore` includes sensitive files

- [ ] **Environment Variables Documented:**
  - [ ] All required variables listed in [EnvironmentVariables.md](EnvironmentVariables.md)
  - [ ] Connection strings prepared (but not committed)
  - [ ] Azure Key Vault endpoint configured

- [ ] **Secrets Management:**
  - [ ] Azure Key Vault created
  - [ ] Secrets added to Key Vault
  - [ ] Managed identity configured
  - [ ] Key Vault access tested

- [ ] **Health Checks:**
  - [ ] Health check endpoints tested locally
  - [ ] `/health`, `/health/ready`, `/health/live` all respond
  - [ ] Database check passes
  - [ ] Azure Storage check passes (if configured)

### Deployment

- [ ] **Environment Configuration:**
  - [ ] `ASPNETCORE_ENVIRONMENT=Production` set
  - [ ] Connection strings configured (environment variables or Key Vault)
  - [ ] Azure Key Vault endpoint set
  - [ ] Application Insights connection string configured (optional)

- [ ] **Logging:**
  - [ ] Production logging levels verified (`Warning` default)
  - [ ] Application Insights enabled (optional)
  - [ ] Log retention policies configured

- [ ] **Security:**
  - [ ] HTTPS enforced (default in Program.cs)
  - [ ] HSTS enabled in production (default)
  - [ ] Secrets not exposed in logs
  - [ ] Connection strings secured

### Post-Deployment

- [ ] **Health Checks:**
  - [ ] `/health` endpoint returns `Healthy`
  - [ ] All checks passing (database, storage)
  - [ ] Health check monitoring configured

- [ ] **Functionality:**
  - [ ] Database migrations applied successfully
  - [ ] Database seeding completed
  - [ ] Azure Blob Storage accessible
  - [ ] File uploads working

- [ ] **Monitoring:**
  - [ ] Health check alerts configured
  - [ ] Application Insights data flowing (if configured)
  - [ ] Log aggregation working
  - [ ] Performance baselines established

---

## Troubleshooting

### Health Check Returns Unhealthy

**Symptoms:**
- `/health` endpoint returns `503 Service Unavailable`
- One or more checks showing `Unhealthy` status

**Solutions:**

1. **Check health endpoint for details:**
   ```bash
   curl https://rushtonroots.azurewebsites.net/health | jq
   ```

2. **Database check failing:**
   - Verify connection string is set: `ConnectionStrings__DefaultConnection`
   - Test database connectivity: `sqlcmd -S server -U user -P password`
   - Check firewall rules (Azure SQL)
   - Verify managed identity permissions (if using)

3. **Azure Storage check failing:**
   - Verify connection string is set: `AzureBlobStorage__ConnectionString`
   - Test storage access: Azure Storage Explorer or `az storage` CLI
   - Check storage account exists and is accessible
   - Verify container exists: `rushtonroots-files`

---

### Configuration Not Loading

**Symptoms:**
- Application uses default values instead of production configuration
- Environment variables not recognized

**Solutions:**

1. **Verify `ASPNETCORE_ENVIRONMENT` is set:**
   ```bash
   # In app logs, look for:
   # "Hosting environment: Production"
   
   # Check environment variable
   echo $ASPNETCORE_ENVIRONMENT  # Linux
   $env:ASPNETCORE_ENVIRONMENT   # Windows
   ```

2. **Check environment variable naming:**
   - Use double underscores: `ConnectionStrings__DefaultConnection`
   - NOT colons: `ConnectionStrings:DefaultConnection` (won't work in env vars)

3. **Verify configuration in Azure App Service:**
   ```bash
   az webapp config appsettings list \
     --name rushtonroots-app \
     --resource-group rushtonroots-rg
   ```

---

### Logging Not Working

**Symptoms:**
- No logs appearing in Application Insights
- Console logs not showing

**Solutions:**

1. **Verify logging configuration:**
   ```json
   // appsettings.Production.json should have:
   {
     "Logging": {
       "LogLevel": {
         "Default": "Warning"
       }
     }
   }
   ```

2. **Check Application Insights connection string:**
   ```bash
   # Verify environment variable is set
   echo $APPLICATIONINSIGHTS_CONNECTION_STRING
   ```

3. **Increase logging level temporarily:**
   ```bash
   export Logging__LogLevel__Default=Information
   ```

---

### Azure Key Vault Access Denied

**Symptoms:**
- Application fails to start with Key Vault errors
- `403 Forbidden` when accessing secrets

**Solutions:**

1. **Verify managed identity is enabled:**
   ```bash
   az webapp identity show \
     --name rushtonroots-app \
     --resource-group rushtonroots-rg
   ```

2. **Check RBAC role assignment:**
   ```bash
   az role assignment list \
     --assignee <principal-id> \
     --scope /subscriptions/{sub}/resourceGroups/rushtonroots-rg/providers/Microsoft.KeyVault/vaults/rushtonroots-keyvault
   ```

3. **Verify Key Vault endpoint:**
   ```bash
   echo $AZURE_KEY_VAULT_ENDPOINT
   # Should be: https://rushtonroots-keyvault.vault.azure.net/
   ```

---

## Summary

Phase 7.1 Configuration Management has implemented:

✅ **Production Configuration:**
- `appsettings.Production.json` created with production-optimized logging
- Secrets excluded from configuration files
- Environment variable approach documented

✅ **Health Checks:**
- Database health check configured (EF Core)
- Azure Blob Storage health check configured
- Three endpoints: `/health`, `/health/ready`, `/health/live`
- JSON response format for monitoring integration

✅ **Documentation:**
- [EnvironmentVariables.md](EnvironmentVariables.md) - Comprehensive guide (20KB)
- Azure Key Vault integration documented
- Production deployment checklist provided
- Troubleshooting guide included

✅ **Logging:**
- Production logging optimized (`Warning` level)
- Development logging verbose (`Debug` level)
- Console timestamps for production debugging

✅ **Security Best Practices:**
- No secrets in source control
- Azure Key Vault integration approach documented
- Managed identity recommended
- Environment variable naming conventions established

---

## Related Documentation

- [Environment Variables Guide](EnvironmentVariables.md) - Detailed environment variable documentation
- [Azure Storage Setup](AzureStorageSetup.md) - Azure storage configuration
- [Developer Onboarding](DeveloperOnboarding.md) - Developer setup guide
- [API Documentation](ApiDocumentation.md) - API endpoint documentation

---

**Phase Status:** ✅ **COMPLETE**  
**Build Status:** ✅ 0 Warnings, 0 Errors  
**Test Status:** ✅ 484/484 Passing  
**Document Version:** 1.0  
**Last Updated:** December 2025  
**Next Phase:** 7.2 - Security Hardening
