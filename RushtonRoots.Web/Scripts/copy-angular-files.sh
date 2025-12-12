#!/bin/bash
# Copy hashed Angular build files to non-hashed versions for Razor view references
# This allows views to use consistent filenames (main.js, polyfills.js, etc.)
# while still benefiting from cache-busting hashed filenames in production

set -e

WWWROOT_PATH="${1:-.}"

echo "Creating non-hashed copies of Angular files in $WWWROOT_PATH..."

# Find and copy main.js
MAIN_FILE=$(find "$WWWROOT_PATH" -maxdepth 1 -name "main.*.js" ! -name "*.map" | head -n 1)
if [ -n "$MAIN_FILE" ]; then
    cp -f "$MAIN_FILE" "$WWWROOT_PATH/main.js"
    echo "Copied $(basename "$MAIN_FILE") to main.js"
fi

# Find and copy polyfills.js
POLYFILLS_FILE=$(find "$WWWROOT_PATH" -maxdepth 1 -name "polyfills.*.js" ! -name "*.map" | head -n 1)
if [ -n "$POLYFILLS_FILE" ]; then
    cp -f "$POLYFILLS_FILE" "$WWWROOT_PATH/polyfills.js"
    echo "Copied $(basename "$POLYFILLS_FILE") to polyfills.js"
fi

# Find and copy runtime.js
RUNTIME_FILE=$(find "$WWWROOT_PATH" -maxdepth 1 -name "runtime.*.js" ! -name "*.map" | head -n 1)
if [ -n "$RUNTIME_FILE" ]; then
    cp -f "$RUNTIME_FILE" "$WWWROOT_PATH/runtime.js"
    echo "Copied $(basename "$RUNTIME_FILE") to runtime.js"
fi

# Find and copy styles.css
STYLES_FILE=$(find "$WWWROOT_PATH" -maxdepth 1 -name "styles.*.css" ! -name "*.map" | head -n 1)
if [ -n "$STYLES_FILE" ]; then
    cp -f "$STYLES_FILE" "$WWWROOT_PATH/styles.css"
    echo "Copied $(basename "$STYLES_FILE") to styles.css"
fi

echo "Non-hashed file copies created successfully!"
