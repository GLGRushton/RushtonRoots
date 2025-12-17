# Angular Routing Configuration

## Overview

This document describes the Angular routing configuration for the RushtonRoots application. The routing module supports both the current architecture (Angular Elements embedded in Razor views) and a future full SPA migration.

**Last Updated**: December 17, 2025  
**Status**: Phase 12.2 Complete

---

## Architecture Context

### Current State

The RushtonRoots application currently uses a hybrid architecture:
- **Server-side**: ASP.NET Core MVC with Razor views
- **Client-side**: Angular components embedded as Angular Elements
- **Navigation**: Primarily uses ASP.NET Core MVC routing (`window.location.href`)

### Routing Module Purpose

The Angular routing module (`app-routing.module.ts`) has been created to:
1. Support a future migration to a full SPA architecture
2. Provide client-side routing for specific features when needed
3. Handle 404 errors gracefully
4. Enable lazy loading for performance optimization
5. Implement route guards for authentication and authorization

### Compatibility

All routes use **hash-based routing** (`useHash: true`) to ensure compatibility with ASP.NET Core MVC. This means Angular routes look like: `/#/people` instead of `/people`.

---

## Route Configuration

### Main Routes

| Route | Component/Module | Lazy Loaded | Description |
|-------|-----------------|-------------|-------------|
| `/` | Redirects to `/home` | No | Root redirect |
| `/home` | HomeModule | Yes | Home page |
| `/account` | AuthModule | Yes | Account routes (login, profile, etc.) |
| `/people` | PersonModule | Yes | Person management |
| `/households` | HouseholdModule | Yes | Household management |
| `/partnerships` | PartnershipModule | Yes | Partnership management |
| `/relationships` | ParentChildModule | Yes | Parent-child relationships |
| `/wiki` | WikiModule | Yes | Wiki articles |
| `/recipes` | ContentModule | Yes | Recipes |
| `/stories` | ContentModule | Yes | Stories |
| `/traditions` | ContentModule | Yes | Traditions |
| `/calendar` | CalendarModule | Yes | Calendar view |
| `/events` | CalendarModule | Yes | Event management |
| `/gallery` | MediaGalleryModule | Yes | Photo gallery |
| `/media` | MediaGalleryModule | Yes | Media management |
| `/not-found` | NotFoundComponent | No | 404 error page |
| `/**` | Redirects to `/not-found` | No | Fallback for unknown routes |

### Router Configuration Options

```typescript
RouterModule.forRoot(routes, {
  // Enable router tracing in development
  enableTracing: false,
  
  // Use hash-based routing for compatibility with ASP.NET Core MVC
  useHash: true,
  
  // Scroll to top on navigation
  scrollPositionRestoration: 'top',
  
  // Anchor scrolling for hash fragments
  anchorScrolling: 'enabled',
  
  // Relative link resolution
  relativeLinkResolution: 'corrected'
})
```

---

## Route Guards

### AuthGuard

**Purpose**: Protects routes that require user authentication.

**Usage**:
```typescript
{
  path: 'profile',
  component: UserProfileComponent,
  canActivate: [AuthGuard]
}
```

**Behavior**:
- Checks if user has authentication cookie (`.AspNetCore.Identity.Application`)
- Redirects to `/Account/Login?returnUrl=...` if not authenticated
- Stores attempted URL for redirect after login

**Implementation**: `/src/app/shared/guards/auth.guard.ts`

### RoleGuard

**Purpose**: Protects routes that require specific user roles (Admin, HouseholdAdmin).

**Usage**:
```typescript
{
  path: 'admin',
  component: AdminComponent,
  canActivate: [RoleGuard],
  data: { roles: ['Admin'] }
}
```

**Multi-role example**:
```typescript
{
  path: 'manage-household',
  component: ManageHouseholdComponent,
  canActivate: [RoleGuard],
  data: { roles: ['Admin', 'HouseholdAdmin'] }
}
```

**Behavior**:
- Checks user role from meta tags or data attributes
- Admin role has access to all protected routes
- HouseholdAdmin role has access to household-specific routes
- Redirects to `/Account/AccessDenied` if user lacks required role

**Implementation**: `/src/app/shared/guards/role.guard.ts`

### UnsavedChangesGuard

**Purpose**: Warns users before navigating away from forms with unsaved changes.

**Component Implementation**:
```typescript
export class PersonFormComponent implements CanComponentDeactivate {
  form: FormGroup;

  canDeactivate(): boolean {
    if (this.form.dirty) {
      return confirm('You have unsaved changes. Do you really want to leave?');
    }
    return true;
  }
}
```

**Route Configuration**:
```typescript
{
  path: 'edit/:id',
  component: PersonFormComponent,
  canDeactivate: [UnsavedChangesGuard]
}
```

**Behavior**:
- Calls component's `canDeactivate()` method before navigation
- Component controls whether navigation proceeds
- Typical pattern: check `form.dirty` and show confirmation dialog

**Implementation**: `/src/app/shared/guards/unsaved-changes.guard.ts`

---

## Lazy Loading

### What is Lazy Loading?

Lazy loading loads feature modules only when their routes are accessed, reducing initial bundle size and improving performance.

### Configured Lazy Loaded Modules

- **HomeModule**: Home page and welcome content
- **AuthModule**: Authentication and account management
- **PersonModule**: Person CRUD operations
- **HouseholdModule**: Household management
- **PartnershipModule**: Partnership relationships
- **ParentChildModule**: Parent-child relationships
- **WikiModule**: Wiki articles and content
- **ContentModule**: Recipes, stories, traditions
- **CalendarModule**: Calendar and events
- **MediaGalleryModule**: Photo and media management

### Syntax

```typescript
{
  path: 'people',
  loadChildren: () => import('./person/person.module').then(m => m.PersonModule)
}
```

---

## 404 Not Found Page

### Component

**Location**: `/src/app/shared/components/not-found/not-found.component.ts`

**Features**:
- User-friendly error message
- Helpful suggestions (check URL, go back, search, etc.)
- Action buttons:
  - Go to Home
  - Go Back
  - Browse People
  - Search
- Contact support link
- Fully responsive Material Design
- Accessible with ARIA labels and keyboard navigation

### Route Configuration

```typescript
{
  path: 'not-found',
  component: NotFoundComponent
},
{
  path: '**',
  redirectTo: '/not-found'
}
```

**Note**: The wildcard route (`**`) must be the **last route** in the configuration.

---

## Route Resolvers

### Purpose

Route resolvers pre-load data before a route is activated, ensuring the component has all necessary data when it initializes.

### Future Implementation

Route resolvers will be implemented for:
- **PersonResolver**: Pre-load person data by ID
- **HouseholdResolver**: Pre-load household data by ID
- **PartnershipResolver**: Pre-load partnership data by ID
- **WikiArticleResolver**: Pre-load wiki article by slug

### Example Implementation

```typescript
@Injectable({
  providedIn: 'root'
})
export class PersonResolver implements Resolve<Person> {
  constructor(private personService: PersonService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Person> {
    const id = route.paramMap.get('id');
    return this.personService.getById(Number(id));
  }
}
```

**Route Configuration**:
```typescript
{
  path: 'people/:id',
  component: PersonDetailsComponent,
  resolve: { person: PersonResolver }
}
```

**Component Usage**:
```typescript
ngOnInit() {
  this.person = this.route.snapshot.data['person'];
}
```

---

## Route Animations

### Purpose

Route animations provide smooth transitions between pages, improving user experience.

### Future Implementation

Define animations in `route-animations.ts`:

```typescript
export const fadeAnimation =
  trigger('routeAnimations', [
    transition('* <=> *', [
      query(':enter, :leave', [
        style({
          position: 'absolute',
          left: 0,
          width: '100%',
          opacity: 0,
          transform: 'scale(0.95) translateY(10px)',
        }),
      ], { optional: true }),
      
      query(':enter', [
        animate('300ms ease-in', style({ opacity: 1, transform: 'scale(1) translateY(0)' })),
      ], { optional: true })
    ]),
  ]);
```

**Apply to Router Outlet**:
```html
<div [@routeAnimations]="prepareRoute(outlet)">
  <router-outlet #outlet="outlet"></router-outlet>
</div>
```

---

## Migration from MVC to SPA Routing

### Current Hybrid Approach

Most navigation uses `window.location.href` to navigate to ASP.NET Core MVC routes:

```typescript
// Current approach
goToPerson(id: number): void {
  window.location.href = `/Person/Details/${id}`;
}
```

### Future SPA Approach

When migrating to full SPA, replace with Angular router navigation:

```typescript
// Future approach
goToPerson(id: number): void {
  this.router.navigate(['/people', id]);
}
```

### Migration Steps

1. **Create component routes** for all features
2. **Implement route guards** for authentication/authorization
3. **Add route resolvers** for data pre-loading
4. **Update navigation** from `window.location.href` to `router.navigate()`
5. **Configure server-side** to serve `index.html` for all routes
6. **Update build process** to use Angular routing
7. **Test all routes** and navigation flows

---

## Testing Routes

### Unit Testing Route Guards

```typescript
describe('AuthGuard', () => {
  let guard: AuthGuard;
  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuthGuard]
    });
    guard = TestBed.inject(AuthGuard);
    router = TestBed.inject(Router);
  });

  it('should allow authenticated users', () => {
    // Mock authentication
    spyOn(guard as any, 'checkAuthentication').and.returnValue(true);
    
    const result = guard.canActivate({} as any, {} as any);
    expect(result).toBe(true);
  });

  it('should redirect unauthenticated users', () => {
    // Mock no authentication
    spyOn(guard as any, 'checkAuthentication').and.returnValue(false);
    spyOn(window.location, 'href', 'set');
    
    guard.canActivate({} as any, { url: '/protected' } as any);
    expect(window.location.href).toContain('/Account/Login');
  });
});
```

### E2E Testing Routes

```typescript
describe('Routing', () => {
  it('should navigate to person details', async () => {
    await page.goto('http://localhost:4200/#/people/1');
    await expect(page.locator('h1')).toContainText('Person Details');
  });

  it('should show 404 for unknown routes', async () => {
    await page.goto('http://localhost:4200/#/unknown-route');
    await expect(page.locator('h1')).toContainText('Page Not Found');
  });

  it('should redirect unauthenticated users', async () => {
    await page.goto('http://localhost:4200/#/profile');
    await expect(page).toHaveURL(/\/Account\/Login/);
  });
});
```

---

## Best Practices

### 1. **Always Use Lazy Loading**

Lazy load feature modules to reduce initial bundle size:

```typescript
// Good ✅
{
  path: 'people',
  loadChildren: () => import('./person/person.module').then(m => m.PersonModule)
}

// Avoid ❌
{
  path: 'people',
  component: PersonIndexComponent
}
```

### 2. **Implement Route Guards for Security**

Always protect sensitive routes with guards:

```typescript
{
  path: 'admin',
  component: AdminComponent,
  canActivate: [AuthGuard, RoleGuard],
  data: { roles: ['Admin'] }
}
```

### 3. **Use Route Resolvers for Data Loading**

Pre-load data to avoid component flash of empty state:

```typescript
{
  path: 'people/:id',
  component: PersonDetailsComponent,
  resolve: { person: PersonResolver }
}
```

### 4. **Implement Unsaved Changes Guards for Forms**

Prevent accidental data loss:

```typescript
{
  path: 'edit/:id',
  component: PersonFormComponent,
  canDeactivate: [UnsavedChangesGuard]
}
```

### 5. **Keep Wildcard Route Last**

The wildcard route must be the last route:

```typescript
const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  // ... other routes
  { path: '**', redirectTo: '/not-found' } // Always last!
];
```

### 6. **Use Descriptive Route Paths**

Use clear, RESTful route paths:

```typescript
// Good ✅
/people/:id
/people/new
/people/:id/edit
/households/:id/members

// Avoid ❌
/person-details/:id
/create-person
/edit-person/:id
```

### 7. **Document Route Data**

Document required route data for guards and resolvers:

```typescript
{
  path: 'admin',
  component: AdminComponent,
  canActivate: [RoleGuard],
  data: { 
    roles: ['Admin'],  // Required roles
    title: 'Admin Dashboard',  // Page title
    breadcrumb: 'Admin'  // Breadcrumb label
  }
}
```

---

## Future Enhancements

### Phase 12.3: Route Animations

- [ ] Define route transition animations
- [ ] Apply animations to router outlet
- [ ] Test animations across routes

### Phase 12.4: Advanced Route Resolvers

- [ ] Implement PersonResolver
- [ ] Implement HouseholdResolver
- [ ] Implement PartnershipResolver
- [ ] Implement WikiArticleResolver

### Phase 12.5: Preloading Strategies

- [ ] Implement custom preloading strategy
- [ ] Preload frequently accessed routes
- [ ] Monitor performance impact

### Phase 12.6: Route Metadata

- [ ] Add page titles to routes
- [ ] Add breadcrumb labels to routes
- [ ] Add SEO metadata to routes

---

## Troubleshooting

### Issue: Routes not working

**Solution**: Ensure `AppRoutingModule` is imported in `app.module.ts` **after** `BrowserModule` but **before** feature modules.

### Issue: 404 page not showing

**Solution**: Check that wildcard route (`**`) is the last route in configuration.

### Issue: Guards not firing

**Solution**: Ensure guards are provided in the module or use `providedIn: 'root'` in guard decorator.

### Issue: Lazy loaded modules not loading

**Solution**: Check module path in `loadChildren` and ensure module has routing configured.

### Issue: Hash routing not working

**Solution**: Verify `useHash: true` is set in `RouterModule.forRoot()` options.

---

## Resources

- [Angular Router Documentation](https://angular.io/guide/router)
- [Route Guards](https://angular.io/guide/router#preventing-unauthorized-access)
- [Lazy Loading](https://angular.io/guide/lazy-loading-ngmodules)
- [Route Resolvers](https://angular.io/api/router/Resolve)
- [Route Animations](https://angular.io/guide/route-animations)

---

**Document Version**: 1.0  
**Last Updated**: December 17, 2025  
**Next Review**: January 2026
