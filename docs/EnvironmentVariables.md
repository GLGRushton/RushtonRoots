# RushtonRoots Environment Variables

**Date:** December 2025  
**Version:** 1.0  
**Status:** Production Configuration Guide

---

## Table of Contents

1. [Overview](#overview)
2. [Required Environment Variables](#required-environment-variables)
3. [Optional Environment Variables](#optional-environment-variables)
4. [Environment-Specific Configuration](#environment-specific-configuration)
5. [Azure Key Vault Integration](#azure-key-vault-integration)
6. [Security Best Practices](#security-best-practices)
7. [Deployment Examples](#deployment-examples)

---

## Overview

RushtonRoots uses environment variables to securely configure sensitive settings without storing them in source control. This document outlines all environment variables used by the application.

### Configuration Hierarchy

Configuration values are loaded in this order (later sources override earlier ones):

1. `appsettings.json` - Base configuration (committed to source control)
2. `appsettings.{Environment}.json` - Environment-specific overrides (committed to source control)
3. **Environment Variables** - Runtime configuration (NOT in source control)
4. **Azure Key Vault** - Secrets management (optional, production recommended)

---

## Required Environment Variables

These environment variables **MUST** be set in production environments:

### 1. ASPNETCORE_ENVIRONMENT

**Purpose:** Specifies the runtime environment  
**Required:** Yes  
**Valid Values:** `Development`, `Staging`, `Production`  
**Default:** `Production`

```bash
# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Production

# Windows PowerShell
$env:ASPNETCORE_ENVIRONMENT="Production"

# Docker
ENV ASPNETCORE_ENVIRONMENT=Production
```

---

### 2. ConnectionStrings__DefaultConnection

**Purpose:** SQL Server database connection string  
**Required:** Yes  
**Format:** Standard SQL Server connection string  
**Security:** Contains sensitive credentials - NEVER commit to source control

```bash
# Linux/Mac (single quotes to prevent shell expansion)
export ConnectionStrings__DefaultConnection='Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=RushtonRoots;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

# Windows PowerShell
$env:ConnectionStrings__DefaultConnection="Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=RushtonRoots;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Docker
ENV ConnectionStrings__DefaultConnection="Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=RushtonRoots;..."
```

**Azure SQL Database Example:**
```
Server=tcp:rushtonroots.database.windows.net,1433;Initial Catalog=RushtonRoots;Persist Security Info=False;User ID=rushton_admin;Password=YourSecurePassword123!;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

**Managed Identity (Recommended for Azure):**
```
Server=tcp:rushtonroots.database.windows.net,1433;Initial Catalog=RushtonRoots;Authentication=Active Directory Default;MultipleActiveResultSets=True;Encrypt=True;
```

---

### 3. AzureBlobStorage__ConnectionString

**Purpose:** Azure Blob Storage connection string for file uploads  
**Required:** Yes  
**Format:** Azure Storage connection string  
**Security:** Contains account key - NEVER commit to source control

```bash
# Linux/Mac
export AzureBlobStorage__ConnectionString='DefaultEndpointsProtocol=https;AccountName=yourstorageaccount;AccountKey=youraccountkey;EndpointSuffix=core.windows.net'

# Windows PowerShell
$env:AzureBlobStorage__ConnectionString="DefaultEndpointsProtocol=https;AccountName=yourstorageaccount;AccountKey=youraccountkey;EndpointSuffix=core.windows.net"

# Docker
ENV AzureBlobStorage__ConnectionString="DefaultEndpointsProtocol=https;AccountName=yourstorageaccount;..."
```

**Example:**
```
DefaultEndpointsProtocol=https;AccountName=rushtonrootsstorage;AccountKey=abcd1234...xyz9876==;EndpointSuffix=core.windows.net
```

**Managed Identity (Recommended for Azure App Service):**
```bash
# No connection string needed - use DefaultAzureCredential
# Set this to enable managed identity:
export AzureBlobStorage__UseManagedIdentity=true
export AzureBlobStorage__StorageAccountName=rushtonrootsstorage
```

---

## Optional Environment Variables

### 4. AzureBlobStorage__ContainerName

**Purpose:** Override the blob storage container name  
**Required:** No  
**Default:** `rushtonroots-files` (from appsettings.json)

```bash
export AzureBlobStorage__ContainerName=rushtonroots-files-prod
```

---

### 5. Logging__LogLevel__Default

**Purpose:** Override default logging level  
**Required:** No  
**Default:** `Warning` (Production), `Debug` (Development)  
**Valid Values:** `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`, `None`

```bash
# Increase logging for debugging production issues
export Logging__LogLevel__Default=Information

# Reduce logging for high-performance scenarios
export Logging__LogLevel__Default=Error
```

---

### 6. APPLICATIONINSIGHTS_CONNECTION_STRING

**Purpose:** Azure Application Insights connection string for telemetry  
**Required:** No (but recommended for production)  
**Format:** Application Insights connection string

```bash
export APPLICATIONINSIGHTS_CONNECTION_STRING='InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://region.in.applicationinsights.azure.com/;LiveEndpoint=https://region.livediagnostics.monitor.azure.com/'
```

---

### 7. AZURE_KEY_VAULT_ENDPOINT

**Purpose:** Azure Key Vault URL for secrets management  
**Required:** No (but recommended for production)  
**Format:** `https://{vault-name}.vault.azure.net/`

```bash
export AZURE_KEY_VAULT_ENDPOINT=https://rushtonroots-keyvault.vault.azure.net/
```

**Note:** When Azure Key Vault is configured, connection strings and secrets can be stored in Key Vault instead of environment variables.

---

## Environment-Specific Configuration

### Development Environment

Development uses `appsettings.Development.json` with local database and Azurite emulator:

```bash
export ASPNETCORE_ENVIRONMENT=Development
# Connection string and Azure storage from appsettings.Development.json
```

**appsettings.Development.json includes:**
- LocalDB connection: `Server=(localdb)\\mssqllocaldb;...`
- Azurite emulator: `UseDevelopmentStorage=true`
- Verbose logging: `Debug` level

---

### Staging Environment

Staging should mirror production but use separate resources:

```bash
export ASPNETCORE_ENVIRONMENT=Staging
export ConnectionStrings__DefaultConnection='Server=tcp:rushtonroots-staging.database.windows.net,...'
export AzureBlobStorage__ConnectionString='DefaultEndpointsProtocol=https;AccountName=rushtonrootsstaging;...'
export AzureBlobStorage__ContainerName=rushtonroots-files-staging
export Logging__LogLevel__Default=Information
```

---

### Production Environment

Production uses minimal logging and secure connection strings:

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection='Server=tcp:rushtonroots-prod.database.windows.net,...'
export AzureBlobStorage__ConnectionString='DefaultEndpointsProtocol=https;AccountName=rushtonrootsprod;...'
export AZURE_KEY_VAULT_ENDPOINT=https://rushtonroots-keyvault.vault.azure.net/
export APPLICATIONINSIGHTS_CONNECTION_STRING='InstrumentationKey=...;...'
export Logging__LogLevel__Default=Warning
```

---

## Azure Key Vault Integration

### Overview

Azure Key Vault is the recommended approach for managing secrets in production. It provides:

- ✅ Centralized secrets management
- ✅ Audit logging of secret access
- ✅ Automatic secret rotation
- ✅ Role-based access control (RBAC)
- ✅ Encryption at rest and in transit

### Setup Steps

#### 1. Create Azure Key Vault

```bash
# Azure CLI
az keyvault create \
  --name rushtonroots-keyvault \
  --resource-group rushtonroots-rg \
  --location eastus \
  --enable-rbac-authorization
```

#### 2. Add Secrets to Key Vault

```bash
# Add database connection string
az keyvault secret set \
  --vault-name rushtonroots-keyvault \
  --name ConnectionStrings--DefaultConnection \
  --value "Server=tcp:rushtonroots.database.windows.net,1433;..."

# Add Azure Blob Storage connection string
az keyvault secret set \
  --vault-name rushtonroots-keyvault \
  --name AzureBlobStorage--ConnectionString \
  --value "DefaultEndpointsProtocol=https;AccountName=rushtonrootsstorage;..."
```

**Note:** Use double dashes (`--`) instead of colons (`:`) in secret names for ASP.NET Core configuration binding.

#### 3. Configure Managed Identity

Enable managed identity for your App Service or VM:

```bash
# Enable system-assigned managed identity for App Service
az webapp identity assign \
  --name rushtonroots-app \
  --resource-group rushtonroots-rg
```

Grant the managed identity access to Key Vault:

```bash
# Get the managed identity principal ID
PRINCIPAL_ID=$(az webapp identity show \
  --name rushtonroots-app \
  --resource-group rushtonroots-rg \
  --query principalId -o tsv)

# Grant Key Vault Secrets User role
az role assignment create \
  --assignee $PRINCIPAL_ID \
  --role "Key Vault Secrets User" \
  --scope /subscriptions/{subscription-id}/resourceGroups/rushtonroots-rg/providers/Microsoft.KeyVault/vaults/rushtonroots-keyvault
```

#### 4. Update Program.cs (Already Documented)

See section below for code implementation.

### Code Implementation

Add Azure Key Vault configuration to `Program.cs`:

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
```

**NuGet Package Required:**
```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

### Using Managed Identity for Azure Storage

When using managed identity, update `BlobStorageService` to use `DefaultAzureCredential`:

```csharp
// In BlobStorageService.cs constructor
var useManagedIdentity = configuration.GetValue<bool>("AzureBlobStorage:UseManagedIdentity");
if (useManagedIdentity)
{
    var storageAccountName = configuration["AzureBlobStorage:StorageAccountName"];
    var blobServiceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
    _blobServiceClient = new BlobServiceClient(blobServiceUri, new DefaultAzureCredential());
}
else
{
    var connectionString = configuration["AzureBlobStorage:ConnectionString"];
    _blobServiceClient = new BlobServiceClient(connectionString);
}
```

---

## Security Best Practices

### ✅ DO

1. **Store secrets in Azure Key Vault** for production environments
2. **Use Managed Identity** instead of connection strings when possible
3. **Rotate secrets regularly** (every 90 days recommended)
4. **Use separate storage accounts** for dev, staging, and production
5. **Enable audit logging** for Key Vault access
6. **Use least privilege principle** - grant minimum required permissions
7. **Monitor secret access** via Azure Monitor and Application Insights

### ❌ DON'T

1. **Never commit secrets** to source control (use `.gitignore`)
2. **Never log connection strings** or secrets
3. **Don't share production credentials** across environments
4. **Don't use the same storage account** for development and production
5. **Don't hardcode secrets** in application code
6. **Don't expose Key Vault endpoint** publicly

### Environment Variable Security

```bash
# ✅ Good - secrets stored in Key Vault or environment variables
export AZURE_KEY_VAULT_ENDPOINT=https://rushtonroots-keyvault.vault.azure.net/

# ❌ Bad - secrets in configuration files
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Password=MyPassword123;" // NEVER DO THIS
  }
}
```

---

## Deployment Examples

### Azure App Service

Set environment variables in Azure Portal:

1. Navigate to **App Service** → **Configuration** → **Application settings**
2. Add new application settings:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ConnectionStrings__DefaultConnection` = `Server=...`
   - `AzureBlobStorage__ConnectionString` = `DefaultEndpointsProtocol=...`
   - `AZURE_KEY_VAULT_ENDPOINT` = `https://rushtonroots-keyvault.vault.azure.net/`
3. Click **Save** and **Restart** the app

**Azure CLI:**
```bash
az webapp config appsettings set \
  --name rushtonroots-app \
  --resource-group rushtonroots-rg \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    AZURE_KEY_VAULT_ENDPOINT=https://rushtonroots-keyvault.vault.azure.net/
```

---

### Docker

Create a `.env` file (add to `.gitignore`):

```bash
# .env file (DO NOT COMMIT)
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=tcp:rushtonroots.database.windows.net,...
AzureBlobStorage__ConnectionString=DefaultEndpointsProtocol=https;...
```

Use environment file with Docker Compose:

```yaml
# docker-compose.yml
version: '3.8'
services:
  web:
    image: rushtonroots:latest
    env_file:
      - .env
    ports:
      - "80:8080"
      - "443:8081"
```

Or pass environment variables directly:

```bash
docker run -d \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Server=..." \
  -e AzureBlobStorage__ConnectionString="DefaultEndpointsProtocol=..." \
  -p 80:8080 \
  rushtonroots:latest
```

---

### Kubernetes

Create a Kubernetes Secret:

```bash
kubectl create secret generic rushtonroots-secrets \
  --from-literal=ConnectionStrings__DefaultConnection="Server=..." \
  --from-literal=AzureBlobStorage__ConnectionString="DefaultEndpointsProtocol=..."
```

Reference in deployment:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: rushtonroots
spec:
  template:
    spec:
      containers:
      - name: web
        image: rushtonroots:latest
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: rushtonroots-secrets
              key: ConnectionStrings__DefaultConnection
        - name: AzureBlobStorage__ConnectionString
          valueFrom:
            secretKeyRef:
              name: rushtonroots-secrets
              key: AzureBlobStorage__ConnectionString
```

---

### GitHub Actions CI/CD

Store secrets in GitHub repository settings:

1. Navigate to **Settings** → **Secrets and variables** → **Actions**
2. Add repository secrets:
   - `AZURE_WEBAPP_PUBLISH_PROFILE`
   - `SQL_CONNECTION_STRING`
   - `AZURE_STORAGE_CONNECTION_STRING`

Use in workflow:

```yaml
# .github/workflows/deploy.yml
- name: Deploy to Azure Web App
  uses: azure/webapps-deploy@v2
  with:
    app-name: rushtonroots-app
    publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
    
- name: Configure App Settings
  run: |
    az webapp config appsettings set \
      --name rushtonroots-app \
      --resource-group rushtonroots-rg \
      --settings \
        ConnectionStrings__DefaultConnection="${{ secrets.SQL_CONNECTION_STRING }}" \
        AzureBlobStorage__ConnectionString="${{ secrets.AZURE_STORAGE_CONNECTION_STRING }}"
```

---

## Configuration Validation

### Startup Validation

Validate required configuration on application startup:

```csharp
// In Program.cs after building configuration
var requiredSettings = new[]
{
    "ConnectionStrings:DefaultConnection",
    "AzureBlobStorage:ConnectionString"
};

foreach (var setting in requiredSettings)
{
    if (string.IsNullOrEmpty(builder.Configuration[setting]))
    {
        throw new InvalidOperationException(
            $"Required configuration '{setting}' is missing. " +
            "Set via environment variable or Azure Key Vault.");
    }
}
```

### Health Check Endpoint

The `/health` endpoint verifies configuration is working:

- Database connection is valid
- Azure Blob Storage is accessible
- All required settings are present

```bash
# Test health endpoint
curl https://rushtonroots.azurewebsites.net/health

# Expected response (healthy):
{
  "status": "Healthy",
  "results": {
    "database": { "status": "Healthy" },
    "azurestorage": { "status": "Healthy" }
  }
}
```

---

## Troubleshooting

### Issue: Configuration Not Loading

**Symptoms:**
- Application uses default/empty connection strings
- Environment variables not recognized

**Solutions:**

1. **Verify environment variable naming:**
   - Use double underscores (`__`) for nested configuration
   - Example: `ConnectionStrings__DefaultConnection` (not `ConnectionStrings:DefaultConnection`)

2. **Check environment variable is set:**
   ```bash
   # Linux/Mac
   printenv | grep ConnectionStrings
   
   # Windows PowerShell
   Get-ChildItem Env: | Where-Object Name -like "*ConnectionStrings*"
   ```

3. **Verify ASPNETCORE_ENVIRONMENT:**
   ```bash
   echo $ASPNETCORE_ENVIRONMENT  # Linux/Mac
   $env:ASPNETCORE_ENVIRONMENT   # Windows PowerShell
   ```

### Issue: Azure Key Vault Access Denied

**Symptoms:**
- `403 Forbidden` errors when accessing secrets
- Application fails to start with Key Vault errors

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
     --scope /subscriptions/{sub-id}/resourceGroups/rushtonroots-rg/providers/Microsoft.KeyVault/vaults/rushtonroots-keyvault
   ```

3. **Verify Key Vault endpoint:**
   ```bash
   echo $AZURE_KEY_VAULT_ENDPOINT
   # Should be: https://{vault-name}.vault.azure.net/
   ```

### Issue: Connection String Format Error

**Symptoms:**
- Database connection failures
- Invalid connection string errors

**Solutions:**

1. **Check for special characters in password:**
   - Escape characters like `;`, `=`, `{`, `}` in connection strings
   - Use single quotes in bash to prevent shell expansion

2. **Validate connection string format:**
   ```bash
   # Azure SQL Database
   Server=tcp:server.database.windows.net,1433;Initial Catalog=dbname;...
   
   # Local SQL Server
   Server=.;Initial Catalog=dbname;Integrated Security=SSPI;...
   ```

3. **Test connection string separately:**
   ```bash
   sqlcmd -S "tcp:rushtonroots.database.windows.net,1433" \
     -d RushtonRoots \
     -U rushton_admin \
     -P "YourPassword"
   ```

---

## Quick Reference

### Common Environment Variables

| Variable | Production Required | Development Default |
|----------|-------------------|---------------------|
| `ASPNETCORE_ENVIRONMENT` | ✅ Yes | `Development` |
| `ConnectionStrings__DefaultConnection` | ✅ Yes | LocalDB |
| `AzureBlobStorage__ConnectionString` | ✅ Yes | Azurite emulator |
| `AzureBlobStorage__ContainerName` | ⚠️ Optional | `rushtonroots-files` |
| `AZURE_KEY_VAULT_ENDPOINT` | ⚠️ Recommended | None |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | ⚠️ Recommended | None |
| `Logging__LogLevel__Default` | ⚠️ Optional | `Warning` |

### Configuration Files Priority

1. `appsettings.json` (base, always loaded)
2. `appsettings.{Environment}.json` (environment-specific)
3. Environment Variables (override config files)
4. Azure Key Vault (override all, if configured)

---

## Related Documentation

- [Azure Storage Setup Guide](AzureStorageSetup.md)
- [Developer Onboarding Guide](DeveloperOnboarding.md)
- [API Documentation](ApiDocumentation.md)
- [Deployment Guide](DeploymentGuide.md) *(to be created in Phase 7.3)*

---

**Document Version:** 1.0  
**Last Updated:** December 2025  
**Next Review:** January 2026  
**Document Owner:** Development Team
