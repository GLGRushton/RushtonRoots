# Angular Build Separation - Migration Guide

## Overview

This document describes the changes made to separate Angular development builds from production deployment, ensuring clean separation between source and build artifacts.

## What Changed

### Before
- Angular built directly to `wwwroot/` on every build
- npm watch process ran automatically during Debug builds
- Development and production files mixed in `wwwroot/`
- node_modules and source files could potentially end up in wwwroot

### After
- Angular builds to `ClientApp/dist/` during development
- `wwwroot/` remains empty until publish or manual copy
- Production builds only copy necessary files to `wwwroot/`
- Clean separation between development and deployment

## Directory Structure

```
RushtonRoots.Web/
├── ClientApp/
│   ├── src/                 # Angular source code
│   ├── dist/                # Angular build output (gitignored)
│   ├── node_modules/        # npm packages (gitignored)
│   ├── angular.json         # Angular CLI config (outputPath: "dist")
│   └── package.json         # npm config
├── Scripts/
│   ├── copy-angular-files.ps1   # Windows: Creates non-hashed copies
│   └── copy-angular-files.sh    # Linux/Mac: Creates non-hashed copies
├── wwwroot/
│   └── .gitkeep            # Empty during development
│                           # Populated only during publish
└── RushtonRoots.Web.csproj  # Updated MSBuild targets
```

## Development Workflow

### Option 1: One-Time Build
```bash
# Build Angular
cd RushtonRoots.Web/ClientApp
npm run build -- --configuration development

# Copy to wwwroot for testing
cd ..
cp -r ClientApp/dist/* wwwroot/

# Run .NET app
dotnet run
```

### Option 2: Watch Mode (Auto-Rebuild)
```bash
# Terminal 1: Watch Angular files
cd RushtonRoots.Web/ClientApp
npm run watch

# Terminal 2: When ready to test, copy files
cd RushtonRoots.Web
cp -r ClientApp/dist/* wwwroot/

# Terminal 3: Run .NET app
cd RushtonRoots.Web
dotnet run
```

### Why Manual Copy?
This ensures you have full control over when Angular changes appear in your running application. It prevents confusion from stale files and makes the development process transparent.

## Production Deployment

### Publishing
```bash
# From repository root
dotnet publish -c Release -o ./publish
```

This automatically:
1. Runs `npm install` in ClientApp
2. Builds Angular with production configuration (minification, optimization, hashing)
3. Copies all files from `ClientApp/dist/` to `wwwroot/`
4. Creates non-hashed copies (main.js, polyfills.js, runtime.js) for Razor view references
5. Includes all wwwroot files in the publish output

### Output Files
After publish, `wwwroot/` contains:
- **Hashed files**: `main.[hash].js`, `polyfills.[hash].js` (for cache-busting)
- **Non-hashed copies**: `main.js`, `polyfills.js` (for Razor view references)
- **Compressed versions**: `.br` and `.gz` files (for performance)

## Razor View Integration

Views now reference non-hashed filenames:

```cshtml
@section Scripts {
    <script src="/runtime.js"></script>
    <script src="/polyfills.js"></script>
    <script src="/main.js"></script>
}
```

These work in both development and production because:
- **Development**: `npm run build -- --configuration development` creates non-hashed files
- **Production**: The copy scripts create non-hashed copies from hashed originals

## Angular Elements

Angular Elements continue to work as before:

```html
<app-family-tree></app-family-tree>
```

The custom elements are bootstrapped when the Angular scripts load.

## MSBuild Integration

### Targets Removed
- **StartNpmWatch**: No longer automatically starts npm watch during Debug builds

### Targets Modified
- **PublishRunWebpack**: Enhanced to copy dist files and create non-hashed copies

### When They Run
- **NpmInstall**: Before any build, if node_modules doesn't exist
- **PublishRunWebpack**: Only during `dotnet publish`, not during `dotnet build`

## Git Tracking

### Ignored Files
The following are now gitignored:
- `ClientApp/dist/` - Angular build output
- `ClientApp/node_modules/` - npm packages
- `wwwroot/*.js`, `wwwroot/*.css`, `wwwroot/*.html` - Angular build artifacts
- `wwwroot/*.map` - Source maps

### Tracked Files
- `wwwroot/.gitkeep` - Keeps the directory in git
- All source files in `ClientApp/src/`
- Configuration files (angular.json, package.json, etc.)

## Troubleshooting

### wwwroot is empty when I run the app
**Solution**: This is expected! Copy files from `ClientApp/dist/` to `wwwroot/`:
```bash
cd RushtonRoots.Web
cp -r ClientApp/dist/* wwwroot/
```

### Angular changes don't appear
**Solution**: Rebuild Angular and copy to wwwroot:
```bash
cd ClientApp && npm run build -- --configuration development
cd .. && cp -r ClientApp/dist/* wwwroot/
```

### Scripts not found (404) in browser
**Solution**: Ensure you've copied the Angular build to wwwroot. Check that files exist:
```bash
ls -la RushtonRoots.Web/wwwroot/
```

### Publish fails
**Solution**: Ensure npm is installed and `ClientApp/package.json` is valid:
```bash
cd RushtonRoots.Web/ClientApp
npm install
npm run build -- --configuration production
```

## Benefits

✅ **Clean Separation**: Development files never touch production deployment
✅ **Explicit Control**: You choose when to copy Angular changes to wwwroot
✅ **No Bloat**: Only production-ready, optimized files in wwwroot
✅ **Git Cleanliness**: Build artifacts never tracked in version control
✅ **Cross-Platform**: Works on Windows, Linux, and Mac
✅ **Cache-Busting**: Production builds use hashed filenames
✅ **Consistent References**: Razor views use stable filenames

## Summary

This change aligns with modern web development best practices:
1. **Separation of Concerns**: Source code separate from build output
2. **Explicit Builds**: Developers control when changes are deployed locally
3. **Production Optimization**: Only optimized, necessary files in deployment
4. **Version Control**: Only source code tracked, not generated files

The workflow may seem more manual, but it provides better control and cleaner project structure.
