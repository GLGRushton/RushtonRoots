# PWA Icons

This directory contains icons for the Progressive Web App (PWA) functionality.

## Required Icons

The following icon sizes are required for PWA support:

- `icon-72x72.png` - Small icon for iOS
- `icon-96x96.png` - Small icon for Android
- `icon-128x128.png` - Medium icon
- `icon-144x144.png` - Medium icon for iOS
- `icon-152x152.png` - Medium icon for iOS
- `icon-192x192.png` - Standard icon for Android
- `icon-384x384.png` - Large icon
- `icon-512x512.png` - Extra large icon for splash screens

## Shortcuts Icons

- `shortcut-family-tree.png` - Icon for family tree shortcut
- `shortcut-add-person.png` - Icon for add person shortcut

## Generating Icons

To generate all required icon sizes from a single source image:

1. Create a high-resolution source image (at least 512x512px)
2. Use an icon generator tool like:
   - https://www.pwabuilder.com/imageGenerator
   - https://realfavicongenerator.net/
   - ImageMagick command line tool

Example using ImageMagick:
```bash
convert icon-source.png -resize 72x72 icon-72x72.png
convert icon-source.png -resize 96x96 icon-96x96.png
convert icon-source.png -resize 128x128 icon-128x128.png
convert icon-source.png -resize 144x144 icon-144x144.png
convert icon-source.png -resize 152x152 icon-152x152.png
convert icon-source.png -resize 192x192 icon-192x192.png
convert icon-source.png -resize 384x384 icon-384x384.png
convert icon-source.png -resize 512x512 icon-512x512.png
```

## Design Guidelines

- Use a simple, recognizable design
- Ensure the icon works well at small sizes
- Use the RushtonRoots green color (#2e7d32) as the primary color
- Make the icon maskable (safe area in the center 80% of the image)
- Test on both light and dark backgrounds
