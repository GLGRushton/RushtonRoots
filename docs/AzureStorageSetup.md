# Azure Blob Storage Setup Guide

This guide explains how to configure Azure Blob Storage for RushtonRoots in both development and production environments.

## Table of Contents
- [Overview](#overview)
- [Development Setup (Azurite)](#development-setup-azurite)
- [Production Setup (Azure Storage Account)](#production-setup-azure-storage-account)
- [Configuration Reference](#configuration-reference)
- [Testing Your Setup](#testing-your-setup)
- [Troubleshooting](#troubleshooting)

---

## Overview

RushtonRoots uses Azure Blob Storage for storing:
- Family photos and thumbnails
- Documents (PDFs, Word docs, etc.)
- Media files (videos, audio recordings)
- User profile pictures

The application automatically generates thumbnails for images in multiple sizes (configurable in `appsettings.json`).

### Storage Organization

Files are organized in the following structure:
```
rushtonroots-files/           # Production container
├── photos/                   # Original photos
├── thumbnails/               
│   ├── small/                # Small thumbnails (200x200)
│   └── medium/               # Medium thumbnails (400x400)
├── documents/                # PDF, Word, Excel files
├── media/                    # Videos, audio files
└── profile-pictures/         # User avatars

rushtonroots-files-dev/       # Development container (same structure)
```

---

## Development Setup (Azurite)

For local development, use **Azurite** - Microsoft's open-source Azure Storage emulator.

### Option 1: Azurite with Docker (Recommended)

**Prerequisites:**
- Docker Desktop installed and running

**Steps:**

1. **Start Azurite container:**
   ```bash
   docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 \
     --name azurite \
     mcr.microsoft.com/azure-storage/azurite
   ```

2. **Verify Azurite is running:**
   ```bash
   docker ps | grep azurite
   ```
   You should see the container running on ports 10000 (blob), 10001 (queue), and 10002 (table).

3. **Configuration is already set** in `appsettings.Development.json`:
   ```json
   {
     "AzureBlobStorage": {
       "ConnectionString": "UseDevelopmentStorage=true",
       "ContainerName": "rushtonroots-files-dev"
     }
   }
   ```

4. **Run the application:**
   ```bash
   cd RushtonRoots.Web
   dotnet run
   ```
   The application will automatically create the container if it doesn't exist.

### Option 2: Azurite with npm (Alternative)

**Prerequisites:**
- Node.js and npm installed

**Steps:**

1. **Install Azurite globally:**
   ```bash
   npm install -g azurite
   ```

2. **Start Azurite:**
   ```bash
   azurite --silent --location c:\azurite --debug c:\azurite\debug.log
   ```
   Or on Linux/Mac:
   ```bash
   azurite --silent --location ~/azurite --debug ~/azurite/debug.log
   ```

3. **Configuration** (same as Option 1 - already set in `appsettings.Development.json`)

4. **Run the application** (same as Option 1)

### Option 3: Visual Studio Azurite Extension

If using Visual Studio 2022+:

1. **Install** the Azurite extension from Extensions > Manage Extensions
2. **Start Azurite** from Tools > Azurite > Start Blob Service
3. **Configuration** (same as above - already set)

### Managing Azurite

**Stop Azurite (Docker):**
```bash
docker stop azurite
```

**Remove Azurite container (Docker):**
```bash
docker rm azurite
```

**View Azurite data:**
Use **Azure Storage Explorer** (free tool from Microsoft):
- Download from: https://azure.microsoft.com/features/storage-explorer/
- Connect to "Local & Attached" > "Storage Accounts" > "(Emulator - Default Ports)"
- Browse containers and files

---

## Production Setup (Azure Storage Account)

### Step 1: Create Azure Storage Account

1. **Sign in to Azure Portal:** https://portal.azure.com

2. **Create a new Storage Account:**
   - Click "Create a resource" > "Storage account"
   - Fill in the details:
     - **Subscription:** Your subscription
     - **Resource Group:** Create new or select existing (e.g., `rg-rushtonroots-prod`)
     - **Storage account name:** `rushtonrootsstorage` (must be globally unique, lowercase, no special chars)
     - **Region:** Choose closest to your users (e.g., `East US`, `West Europe`)
     - **Performance:** Standard (sufficient for most workloads)
     - **Redundancy:** LRS (Locally Redundant Storage) for cost savings, or GRS (Geo-Redundant) for high availability

3. **Advanced settings:**
   - Enable "Blob public access": **Disabled** (security best practice - use SAS tokens instead)
   - Minimum TLS version: **Version 1.2**

4. **Click "Review + Create"** then **"Create"**

### Step 2: Create Blob Container

1. **Navigate to your storage account** in Azure Portal

2. **Go to "Containers"** (under Data storage in left menu)

3. **Click "+ Container":**
   - **Name:** `rushtonroots-files`
   - **Public access level:** Private (no anonymous access)
   - Click **Create**

### Step 3: Get Connection String

**Option A: Using Connection String (Simple but less secure)**

1. In your storage account, go to **"Access keys"** (under Security + networking)
2. Click **"Show keys"**
3. Copy **Connection string** under `key1`

**Important:** Never commit connection strings to source control!

**Option B: Using Azure Key Vault (Recommended for Production)**

1. Create an Azure Key Vault
2. Store the connection string as a secret
3. Configure your app to read from Key Vault using Managed Identity

### Step 4: Configure Application

**For direct deployment (not recommended):**

Update `appsettings.json`:
```json
{
  "AzureBlobStorage": {
    "ConnectionString": "<paste-your-connection-string-here>",
    "ContainerName": "rushtonroots-files"
  }
}
```

**For production (recommended) - Use Environment Variables:**

Set the following environment variables on your hosting platform:

```bash
# Azure App Service
az webapp config appsettings set \
  --name <your-app-name> \
  --resource-group <your-rg-name> \
  --settings AzureBlobStorage__ConnectionString="<your-connection-string>"

# Or via Azure Portal:
# Go to App Service > Configuration > Application settings > New application setting
# Name: AzureBlobStorage__ConnectionString
# Value: <your-connection-string>
```

**For Azure Key Vault integration:**

1. Create a Key Vault and store your connection string as a secret
2. Enable Managed Identity for your App Service
3. Grant the Managed Identity access to Key Vault
4. Reference the secret in your app configuration:

```json
{
  "AzureBlobStorage": {
    "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://<your-vault>.vault.azure.net/secrets/<secret-name>/)"
  }
}
```

---

## Configuration Reference

### appsettings.json Structure

```json
{
  "AzureBlobStorage": {
    "ConnectionString": "<connection-string>",
    "ContainerName": "rushtonroots-files",
    "ThumbnailSizes": [
      { "Name": "small", "Width": 200, "Height": 200 },
      { "Name": "medium", "Width": 400, "Height": 400 }
    ],
    "ThumbnailQuality": 85
  }
}
```

### Configuration Options

| Setting | Description | Development Value | Production Value |
|---------|-------------|-------------------|------------------|
| `ConnectionString` | Azure Storage connection string | `UseDevelopmentStorage=true` | Actual connection string from Azure |
| `ContainerName` | Blob container name | `rushtonroots-files-dev` | `rushtonroots-files` |
| `ThumbnailSizes` | Array of thumbnail dimensions | Same as production | `[{Name: "small", Width: 200, Height: 200}, {Name: "medium", Width: 400, Height: 400}]` |
| `ThumbnailQuality` | JPEG quality (0-100) | 85 | 85 (lower = smaller files, higher = better quality) |

### Environment-Specific Configuration Files

- **appsettings.json** - Base configuration (production defaults)
- **appsettings.Development.json** - Development overrides (uses Azurite)
- **appsettings.Production.json** - Production overrides (optional)

ASP.NET Core automatically loads `appsettings.{Environment}.json` based on the `ASPNETCORE_ENVIRONMENT` variable.

### Environment Variables

You can override any configuration setting using environment variables with the format:
```
Section__Subsection__Setting
```

Examples:
```bash
# Override connection string
export AzureBlobStorage__ConnectionString="UseDevelopmentStorage=true"

# Override container name
export AzureBlobStorage__ContainerName="my-custom-container"

# Override thumbnail quality
export AzureBlobStorage__ThumbnailQuality="90"
```

---

## Testing Your Setup

### 1. Verify Configuration is Loaded

Add logging to verify configuration at startup (already implemented in the application):

```csharp
// In Program.cs or Startup.cs
var blobConfig = builder.Configuration.GetSection("AzureBlobStorage");
Console.WriteLine($"Blob Storage Container: {blobConfig["ContainerName"]}");
Console.WriteLine($"Using Development Storage: {blobConfig["ConnectionString"] == "UseDevelopmentStorage=true"}");
```

### 2. Test File Upload

**Via UI:**
1. Start the application
2. Navigate to photo upload page
3. Select an image and upload
4. Verify the upload succeeds
5. Check that thumbnails are generated

**Via Azure Storage Explorer:**
1. Open Azure Storage Explorer
2. Connect to your storage (local emulator or Azure account)
3. Navigate to your container
4. Verify files appear in the correct folders:
   - Original: `/photos/`
   - Thumbnails: `/thumbnails/small/` and `/thumbnails/medium/`

### 3. Test File Download

1. Navigate to a photo gallery page
2. Click on a photo
3. Verify the image loads correctly
4. Verify thumbnail versions load in gallery views

### 4. Verify Thumbnail Generation

Upload a test image and verify that:
- Original image is saved to `/photos/`
- Small thumbnail (200x200) is created in `/thumbnails/small/`
- Medium thumbnail (400x400) is created in `/thumbnails/medium/`
- All files use the same base name with appropriate paths

---

## Troubleshooting

### Common Issues

#### 1. "No such host is known" or Connection Refused

**Symptom:** Application can't connect to Azurite

**Solutions:**
- Verify Azurite is running: `docker ps | grep azurite`
- If using Docker, ensure container is started: `docker start azurite`
- If using npm, start Azurite: `azurite --silent`
- Check connection string in `appsettings.Development.json` is exactly: `UseDevelopmentStorage=true`

#### 2. "The specified container does not exist"

**Symptom:** Application reports container not found

**Solution:**
- The application should auto-create the container on first run
- Verify `BlobStorageService` has container creation logic
- Manually create container using Azure Storage Explorer
- Check container name matches configuration exactly (case-sensitive)

#### 3. "Blob operation failed" or Upload Errors

**Symptom:** Files fail to upload

**Solutions:**
- Verify connection string is correct
- Check that Azurite/Azure Storage is accessible
- Ensure container exists and app has permissions
- Check disk space (for Azurite)
- Review application logs for detailed error messages

#### 4. Environment Variable Not Overriding Configuration

**Symptom:** Changes to environment variables don't take effect

**Solutions:**
- Restart the application after setting environment variables
- Verify environment variable name uses double underscores: `AzureBlobStorage__ConnectionString`
- Check `ASPNETCORE_ENVIRONMENT` is set correctly (`Development`, `Production`, etc.)
- Ensure `appsettings.{Environment}.json` exists and is valid JSON

#### 5. Production Connection String Exposed in Source Control

**Symptom:** Accidentally committed secrets

**Solutions:**
- **Immediately** rotate the storage account keys in Azure Portal
- Remove the connection string from all configuration files
- Use environment variables or Azure Key Vault instead
- Add `appsettings.Production.json` to `.gitignore` if storing secrets there (not recommended)
- Review commit history and use tools like `git-secrets` or BFG Repo-Cleaner

#### 6. Thumbnails Not Generated

**Symptom:** Original images upload but thumbnails missing

**Solutions:**
- Verify ImageSharp package is installed: `dotnet list package | grep ImageSharp`
- Check `ThumbnailSizes` configuration is valid in `appsettings.json`
- Review application logs for image processing errors
- Ensure uploaded files are valid image formats (JPEG, PNG, GIF)
- Check that `BlobStorageService.GenerateThumbnailsAsync` is being called

#### 7. Slow Upload/Download Performance

**Symptom:** File operations take too long

**Solutions:**
- **For Azurite:** This is normal - emulator is slower than real Azure Storage
- **For Azure Storage:** 
  - Choose a region closer to your users/deployment
  - Consider upgrading to Premium performance tier
  - Implement CDN (Azure CDN or Azure Front Door) for faster global access
  - Optimize image sizes before upload
  - Use thumbnail URLs for gallery views instead of full images

---

## Best Practices

### Security

1. **Never commit connection strings** to source control
2. **Use Azure Key Vault** for production secrets
3. **Enable Managed Identity** for Azure-hosted apps
4. **Use SAS tokens** for temporary access instead of making containers public
5. **Rotate access keys regularly** (every 90 days recommended)
6. **Enable Storage Account firewall** to restrict access to known IPs/VNets
7. **Enable soft delete** for blob recovery (recommended: 7-14 days retention)

### Performance

1. **Use thumbnails** for gallery views (already implemented)
2. **Implement caching** for frequently accessed files
3. **Use CDN** for global distribution of static content
4. **Compress images** before upload when possible
5. **Use async/await** for all storage operations (already implemented)
6. **Set appropriate blob tier** (Hot/Cool/Archive) based on access patterns

### Cost Optimization

1. **Use lifecycle management** to move old files to Cool/Archive storage
2. **Delete old thumbnails** when originals are deleted (already implemented)
3. **Monitor storage usage** with Azure Monitor
4. **Consider blob versioning** only if needed (adds cost)
5. **Use LRS** instead of GRS if geo-redundancy isn't critical
6. **Review blob access patterns** and adjust storage tiers accordingly

### Monitoring

1. **Enable Storage Analytics** in Azure Portal
2. **Set up alerts** for failed operations or high latency
3. **Monitor costs** with Azure Cost Management
4. **Track blob storage metrics** (transactions, data transfer, etc.)
5. **Review logs regularly** for errors and performance issues

---

## Additional Resources

### Microsoft Documentation
- [Azure Blob Storage overview](https://docs.microsoft.com/azure/storage/blobs/storage-blobs-overview)
- [Azurite emulator](https://docs.microsoft.com/azure/storage/common/storage-use-azurite)
- [Azure Storage security guide](https://docs.microsoft.com/azure/storage/common/storage-security-guide)
- [Azure Key Vault integration](https://docs.microsoft.com/aspnet/core/security/key-vault-configuration)

### Tools
- [Azure Storage Explorer](https://azure.microsoft.com/features/storage-explorer/) - Free desktop app for managing storage
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) - Command-line tool for Azure management
- [Azure Storage Blobs client library for .NET](https://docs.microsoft.com/dotnet/api/overview/azure/storage.blobs-readme)

### RushtonRoots Documentation
- [README.md](../README.md) - Project overview and setup
- [PATTERNS.md](../PATTERNS.md) - Architecture and coding patterns
- [CodebaseReviewAndPhasedPlan.md](CodebaseReviewAndPhasedPlan.md) - Implementation roadmap

---

## Quick Reference

### Development (Azurite)
```bash
# Start Azurite with Docker
docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 --name azurite mcr.microsoft.com/azure-storage/azurite

# View Azurite logs
docker logs azurite

# Stop Azurite
docker stop azurite

# Remove Azurite
docker rm azurite
```

### Production Connection String Format
```
DefaultEndpointsProtocol=https;AccountName=<account-name>;AccountKey=<account-key>;EndpointSuffix=core.windows.net
```

### Environment Variable Override
```bash
# Windows (PowerShell)
$env:AzureBlobStorage__ConnectionString = "UseDevelopmentStorage=true"

# Linux/Mac
export AzureBlobStorage__ConnectionString="UseDevelopmentStorage=true"

# Docker
docker run -e AzureBlobStorage__ConnectionString="UseDevelopmentStorage=true" ...
```

---

**Last Updated:** December 2025  
**Document Version:** 1.0  
**Maintained By:** RushtonRoots Development Team
