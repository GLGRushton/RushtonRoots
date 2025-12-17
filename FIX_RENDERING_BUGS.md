# Rendering Bugs Fix

## Issue Summary
The application had two main rendering bugs:
1. **Duplicate Header/Footer**: The app-header was rendering 2 mat-toolbars (showing both logged-in and logged-out states simultaneously), and the app-footer was duplicated
2. **Console Error**: `ERROR TypeError: Cannot read properties of undefined (reading 'name')` at `UserMenuComponent_Template (user-menu.component.html:12:33)`

## Root Cause
The issues were caused by Angular Elements initialization timing problems. When components are converted to custom elements (Web Components), they can render their templates BEFORE the input properties are set from the parent. This caused:

1. **userInfo being undefined/null** during initial render
2. **Both authenticated and unauthenticated UI elements showing** because conditional `*ngIf` directives evaluated before userInfo was properly initialized
3. **TypeError when accessing userInfo.name** in templates

## Changes Made

### 1. UserMenuComponent (`user-menu.component.ts` & `.html`)
**Before:**
```typescript
@Input() userInfo!: UserInfo;  // Non-null assertion - assumes always defined
```
```html
<span class="user-name">{{ userInfo.name }}</span>
```

**After:**
```typescript
@Input() userInfo?: UserInfo;  // Optional - can be undefined
```
```html
<span class="user-name">{{ userInfo?.name || 'User' }}</span>  // Safe navigation with fallback
```

**Changes:**
- Made `userInfo` optional with `?` instead of non-null assertion `!`
- Added optional chaining (`?.`) in template
- Added fallback values (`|| 'User'`, `|| 'Member'`) for when userInfo is undefined
- Applied to both display areas (button and dropdown menu)

### 2. HeaderComponent (`header.component.ts` & `.html`)
**Before:**
```typescript
private _userInfo: UserInfo = {
  name: '',
  role: '',
  isAuthenticated: false,  // Default to false
  isAdmin: false,
  isHouseholdAdmin: false
};

@Input() 
set userinfo(value: string | UserInfo) {
  if (typeof value === 'string') {
    try {
      this._userInfo = JSON.parse(value);
    } catch (e) {
      console.error('Failed to parse userinfo:', e);
    }
  } else {
    this._userInfo = value;
  }
}
```
```html
*ngIf="userInfo.isAuthenticated"
*ngIf="!userInfo.isAuthenticated"
```

**After:**
```typescript
private _userInfo: UserInfo | null = null;  // Start as null

@Input() 
set userinfo(value: string | UserInfo | null) {
  if (!value) {
    this._userInfo = null;
    return;
  }
  
  if (typeof value === 'string') {
    try {
      this._userInfo = JSON.parse(value);
    } catch (e) {
      console.error('Failed to parse userinfo:', e);
      this._userInfo = null;  // Set to null on error
    }
  } else {
    this._userInfo = value;
  }
}

get userInfo(): UserInfo | null {
  return this._userInfo;
}
```
```html
*ngIf="userInfo && userInfo.isAuthenticated"
*ngIf="userInfo && !userInfo.isAuthenticated"
```

**Changes:**
- Changed initial value from default object to `null`
- Added null check in setter to handle undefined/null values
- Added null check guard in ALL `*ngIf` conditions (`userInfo && userInfo.isAuthenticated`)
- This prevents showing BOTH authenticated and unauthenticated UI during initialization

### 3. LayoutWrapperComponent (`layout-wrapper.component.ts`)
**Similar changes to HeaderComponent:**
- Changed `private _userInfo: UserInfo` to `private _userInfo: UserInfo | null = null`
- Updated setter to handle null values explicitly
- Updated getter return type to `UserInfo | null`

## Why This Fixes the Issues

### Duplicate Header/Footer Fix
By starting with `userInfo = null` instead of a default object with `isAuthenticated: false`, and adding explicit null checks (`userInfo && userInfo.isAuthenticated`), we ensure that:

1. **During initialization** (before userInfo is set), the template sees `userInfo === null`
2. **Both conditional blocks** `*ngIf="userInfo && userInfo.isAuthenticated"` and `*ngIf="userInfo && !userInfo.isAuthenticated"` evaluate to `false`
3. **Neither UI element renders** until userInfo is properly set
4. **After userInfo is set**, only ONE of the conditional blocks will render based on the actual authentication state

This prevents the "double rendering" where both the logged-in and logged-out UI elements would appear simultaneously.

### Console Error Fix
By using optional chaining (`userInfo?.name`) and fallback values (`|| 'User'`), we prevent:
- TypeError when trying to read properties of undefined
- Template failing to render
- User seeing broken UI

## Testing
To verify the fix works:
1. Build the Angular application: `cd RushtonRoots.Web/ClientApp && npm run build`
2. Run the application: `cd .. && dotnet run`
3. Navigate to the home page
4. **Expected behavior:**
   - Only ONE header toolbar should be visible
   - Only ONE footer should be visible
   - If logged out: Should see the "Login" button
   - If logged in: Should see the user menu with user name
   - No console errors about undefined properties

## Technical Notes
- These changes align with Angular best practices for components used as Angular Elements
- Optional chaining (`?.`) is standard TypeScript/JavaScript for safe property access
- Null checks in template conditionals prevent premature rendering
- This pattern should be applied to other components that:
  - Are used as Angular Elements (custom elements)
  - Receive complex input objects that might not be immediately available
  - Have conditional rendering based on authentication state

## Related Files
- `RushtonRoots.Web/ClientApp/src/app/shared/components/header/header.component.ts`
- `RushtonRoots.Web/ClientApp/src/app/shared/components/header/header.component.html`
- `RushtonRoots.Web/ClientApp/src/app/shared/components/user-menu/user-menu.component.ts`
- `RushtonRoots.Web/ClientApp/src/app/shared/components/user-menu/user-menu.component.html`
- `RushtonRoots.Web/ClientApp/src/app/shared/components/layout-wrapper/layout-wrapper.component.ts`
- `RushtonRoots.Web/Views/Shared/_Layout.cshtml` (where layout-wrapper is used)
