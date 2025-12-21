# Security Configuration Guide

**Document Version:** 1.0  
**Last Updated:** December 21, 2025  
**Status:** Phase 7.2 Complete

---

## Table of Contents

1. [Overview](#overview)
2. [HTTPS Configuration](#https-configuration)
3. [HSTS (HTTP Strict Transport Security)](#hsts-http-strict-transport-security)
4. [CORS Policy](#cors-policy)
5. [Authorization & Authentication](#authorization--authentication)
6. [Cookie Security](#cookie-security)
7. [Security Scan Results](#security-scan-results)
8. [Production Checklist](#production-checklist)
9. [Troubleshooting](#troubleshooting)

---

## Overview

RushtonRoots implements a comprehensive security configuration following industry best practices:

- ✅ **HTTPS Enforcement**: Required in production with HSTS headers
- ✅ **Authentication**: ASP.NET Core Identity with secure password policies
- ✅ **Authorization**: All API endpoints require authentication (except explicitly public ones)
- ✅ **Cookie Security**: HttpOnly, Secure, SameSite policies enabled
- ✅ **No Vulnerabilities**: Zero vulnerable packages in dependencies
- ✅ **CORS**: Restrictive policy (same-origin by default, configurable if needed)

---

## HTTPS Configuration

### Production Environment

HTTPS is **enforced** in production through:

1. **HTTPS Redirect Middleware**
   ```csharp
   // Program.cs - Production only
   if (!app.Environment.IsDevelopment())
   {
       app.UseHttpsRedirection();
   }
   ```

2. **HSTS Headers** (see next section)

3. **Secure Cookies**
   ```csharp
   options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
   ```

### Development Environment

HTTPS redirect is **optional** in development:
- Enabled by default but can be disabled for local testing without SSL certificates
- To disable, comment out `app.UseHttpsRedirection();` in development block

### SSL Certificate Setup

**Local Development:**
```bash
# Trust the .NET development certificate
dotnet dev-certs https --trust
```

**Production (Azure App Service):**
- SSL certificates are managed by Azure
- Custom domains automatically get free SSL certificates
- Configure in Azure Portal: App Service → TLS/SSL settings

**Production (IIS):**
- Import SSL certificate via IIS Manager
- Bind certificate to site
- Ensure HTTPS binding is on port 443

---

## HSTS (HTTP Strict Transport Security)

### Configuration

HSTS is configured in **production only** with the following settings:

```csharp
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);  // 1 year
    options.IncludeSubDomains = true;          // Apply to all subdomains
    options.Preload = true;                    // Allow preload list inclusion
});
```

### What HSTS Does

1. **Forces HTTPS**: Browser automatically converts all HTTP requests to HTTPS
2. **Prevents Downgrade Attacks**: Blocks man-in-the-middle attacks that try to downgrade to HTTP
3. **Valid for 1 Year**: Browser remembers setting for 365 days
4. **Applies to Subdomains**: All subdomains also use HTTPS

### HSTS Headers Sent

```
Strict-Transport-Security: max-age=31536000; includeSubDomains; preload
```

### HSTS Preload List

To submit your domain to the HSTS preload list (optional but recommended):

1. Ensure HSTS is enabled with `Preload = true`
2. Ensure max-age is at least 31536000 seconds (1 year)
3. Visit https://hstspreload.org/
4. Submit your domain
5. Wait for inclusion (can take several weeks)

**Benefits:**
- Browsers enforce HTTPS even on first visit
- Enhanced security for all users
- Protection against SSL stripping attacks

**Considerations:**
- **Irreversible** for preload list (difficult to remove)
- All subdomains must support HTTPS
- Test thoroughly before submitting

---

## CORS Policy

### Default Configuration (Same-Origin)

By default, CORS is **not required** because:
- Angular frontend is served from the same origin as the API
- All requests are same-origin by default
- Browser's same-origin policy provides adequate security

### Enabling CORS (If Needed)

If you need to allow external origins to access your API (e.g., mobile app, external integrations):

1. **Uncomment CORS configuration in Program.cs:**

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

2. **Enable CORS middleware:**

```csharp
app.UseCors("AllowSpecificOrigins");
```

### CORS Best Practices

**DO:**
- ✅ Specify exact origins (avoid wildcards in production)
- ✅ Use HTTPS origins only
- ✅ Enable credentials only if needed
- ✅ Restrict methods/headers if possible

**DON'T:**
- ❌ Use `AllowAnyOrigin()` in production
- ❌ Allow HTTP origins in production
- ❌ Combine `AllowAnyOrigin()` with `AllowCredentials()`

### Example Configurations

**Mobile App:**
```csharp
policy.WithOrigins("rushtonroots://app")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials();
```

**Trusted Partner Integration:**
```csharp
policy.WithOrigins("https://partner.example.com")
      .WithMethods("GET", "POST")
      .WithHeaders("Authorization", "Content-Type")
      .AllowCredentials();
```

**Public Read-Only API:**
```csharp
policy.AllowAnyOrigin()
      .WithMethods("GET", "OPTIONS")
      .WithHeaders("Accept");
// Note: Cannot use AllowCredentials() with AllowAnyOrigin()
```

---

## Authorization & Authentication

### Authentication Implementation

RushtonRoots uses **ASP.NET Core Identity** for authentication:

```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    options.User.RequireUniqueEmail = true;
});
```

### Password Requirements

- **Minimum Length**: 8 characters
- **Uppercase**: Required
- **Lowercase**: Required
- **Digit**: Required
- **Special Character**: Not required (for better user experience)

### Account Lockout

- **Lockout Duration**: 5 minutes
- **Max Failed Attempts**: 5
- **Applies To**: All users (including new users)

### Authorization on Controllers

**All API controllers require authentication** except those explicitly marked with `[AllowAnonymous]`:

#### Authenticated Endpoints (Default)

All controllers in `/Controllers/Api/` have `[Authorize]` attribute:
- PersonApiController
- HouseholdController
- ParentChildController
- PartnershipController
- PhotoController
- PhotoAlbumController
- DocumentController
- MediaController
- StoryController
- TraditionController
- RecipeController
- WikiPageController
- ChatRoomController
- MessageController
- NotificationController
- FamilyEventController
- FamilyTaskController
- ActivityFeedController
- ContributionController
- LeaderboardController
- And more...

#### Public Endpoints (Explicitly Allowed)

Only these controllers allow anonymous access:

1. **SampleApiController** (`[AllowAnonymous]`)
   - Test endpoint for health checks
   - Returns timestamp and simple messages
   - Safe to be public
   - Can be removed in production if not needed

2. **Health Check Endpoints**
   - `/health` - Full health status
   - `/health/ready` - Readiness probe
   - `/health/live` - Liveness probe
   - Used by monitoring systems

### Role-Based Authorization

Some endpoints require specific roles:

```csharp
[Authorize(Roles = "Admin,HouseholdAdmin")]
public async Task<IActionResult> UpdateMemberRole(...)
```

**Roles:**
- **Admin**: System administrators
- **HouseholdAdmin**: Household administrators
- **User**: Regular users (default)

### Reviewing Authorization

To audit all endpoints:

```bash
# Find controllers without [Authorize]
grep -L "^\[Authorize" RushtonRoots.Web/Controllers/Api/*.cs

# Find controllers with [AllowAnonymous]
grep -l "^\[AllowAnonymous" RushtonRoots.Web/Controllers/Api/*.cs
```

---

## Cookie Security

### Configuration

Authentication cookies are configured with multiple security layers:

```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    // Authentication paths
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    
    // Cookie lifetime
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
    
    // Security settings
    options.Cookie.SecurePolicy = builder.Environment.IsProduction() 
        ? CookieSecurePolicy.Always 
        : CookieSecurePolicy.SameAsRequest;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
```

### Security Features

1. **Secure Policy (Production)**
   - `CookieSecurePolicy.Always`: Cookies only sent over HTTPS
   - Prevents cookie theft over insecure connections
   - Development: `SameAsRequest` for flexibility

2. **HttpOnly**
   - `true`: Prevents client-side JavaScript access
   - Protects against XSS (Cross-Site Scripting) attacks
   - Cookie only accessible via HTTP(S) requests

3. **SameSite**
   - `SameSiteMode.Lax`: Prevents CSRF (Cross-Site Request Forgery)
   - Cookies sent on top-level navigation
   - Not sent on cross-site subrequests (images, iframes)

4. **Sliding Expiration**
   - Cookie renewed on each request
   - 14-day validity extended with activity
   - Automatic logout after 14 days of inactivity

---

## Security Scan Results

### Package Vulnerability Scan

**Status:** ✅ **PASSING** (0 vulnerabilities)

```bash
dotnet list package --vulnerable
```

**Results (December 21, 2025):**
```
The given project `RushtonRoots.Web` has no vulnerable packages
The given project `RushtonRoots.Domain` has no vulnerable packages
The given project `RushtonRoots.Infrastructure` has no vulnerable packages
The given project `RushtonRoots.Application` has no vulnerable packages
The given project `RushtonRoots.UnitTests` has no vulnerable packages
```

### Previous Vulnerabilities (Resolved)

| Package | Version (Vulnerable) | Version (Fixed) | Advisory | Resolved |
|---------|---------------------|-----------------|----------|----------|
| System.Security.Cryptography.Xml | 4.5.0 | 10.0.1 | GHSA-vh55-786g-wjwj | Phase 1.2 ✅ |

### Ongoing Security Monitoring

**Automated Checks:**
- GitHub Dependabot enabled (monitors dependencies)
- Security advisories reviewed weekly
- Update dependencies quarterly (or when vulnerabilities found)

**Manual Checks:**
```bash
# Check for vulnerable packages
dotnet list package --vulnerable

# Check for outdated packages
dotnet list package --outdated

# Update to latest secure versions
dotnet update
```

---

## Production Checklist

Use this checklist before deploying to production:

### SSL/TLS Configuration
- [ ] SSL certificate installed and valid
- [ ] HTTPS binding configured (port 443)
- [ ] HTTP to HTTPS redirect working
- [ ] HSTS headers being sent
- [ ] Certificate expiry monitoring in place

### Environment Configuration
- [ ] `ASPNETCORE_ENVIRONMENT=Production` set
- [ ] Connection strings secured (Azure Key Vault or environment variables)
- [ ] Azure Blob Storage connection secured
- [ ] No secrets in appsettings.json

### Authentication & Authorization
- [ ] All API endpoints have `[Authorize]` or `[AllowAnonymous]`
- [ ] Test endpoints removed or secured (SampleApiController)
- [ ] Strong password policy enforced
- [ ] Account lockout configured
- [ ] Email confirmation enabled (if applicable)

### Cookie Security
- [ ] `Cookie.SecurePolicy = Always` in production
- [ ] `Cookie.HttpOnly = true`
- [ ] `Cookie.SameSite = Lax` or `Strict`

### CORS (If Enabled)
- [ ] Only specific origins allowed (no wildcards)
- [ ] HTTPS origins only
- [ ] Credentials only if necessary
- [ ] Methods/headers restricted

### Security Scans
- [ ] `dotnet list package --vulnerable` returns 0 vulnerabilities
- [ ] No security warnings in build
- [ ] All 484+ tests passing

### Monitoring
- [ ] Health checks working (`/health`, `/health/ready`, `/health/live`)
- [ ] Logging configured (Warning level in production)
- [ ] Security events logged (failed logins, authorization failures)

### Headers & Policies
- [ ] HSTS header present: `Strict-Transport-Security: max-age=31536000`
- [ ] Security headers configured (Content-Security-Policy, X-Frame-Options)
- [ ] Anti-forgery tokens validated on POST/PUT/DELETE

### Database
- [ ] Connection string encrypted
- [ ] Database firewall configured (Azure SQL)
- [ ] SQL injection protection verified (EF Core parameterized queries)

---

## Troubleshooting

### HTTPS Redirect Loop

**Problem:** Infinite redirect between HTTP and HTTPS

**Solution:**
1. Check if load balancer/reverse proxy terminates SSL
2. Configure forwarded headers:
   ```csharp
   app.UseForwardedHeaders(new ForwardedHeadersOptions
   {
       ForwardedHeaders = ForwardedHeaders.XForwardedProto
   });
   ```
3. Place before `app.UseHttpsRedirection()`

### HSTS Not Working

**Problem:** HSTS headers not being sent

**Checklist:**
- [ ] HSTS only works over HTTPS (not HTTP)
- [ ] Must be in production environment
- [ ] Check response headers: `Strict-Transport-Security`
- [ ] Clear browser cache (HSTS is cached)

### CORS Errors in Browser

**Problem:** "CORS policy: No 'Access-Control-Allow-Origin' header"

**Solutions:**
1. For same-origin: Ensure Angular and API on same domain
2. For cross-origin: Enable CORS policy in Program.cs
3. Check origin matches exactly (protocol, domain, port)
4. Verify CORS middleware order (before UseAuthorization)

### Cookie Not Being Set

**Problem:** Authentication cookie not persisted

**Checklist:**
- [ ] HTTPS enabled (if `SecurePolicy = Always`)
- [ ] SameSite policy allows the scenario
- [ ] Browser allows cookies (not in incognito/private mode)
- [ ] Cookie domain matches request domain

### Authorization Failing

**Problem:** 401 Unauthorized on API requests

**Solutions:**
1. Check if user is authenticated: `User.Identity.IsAuthenticated`
2. Verify cookie is being sent (check browser DevTools)
3. Check authorization attribute on controller/action
4. Verify role membership if using role-based authorization

### Security Scan Warnings

**Problem:** Vulnerable packages detected

**Solution:**
```bash
# List vulnerable packages
dotnet list package --vulnerable

# Update specific package
dotnet add package <PackageName> --version <NewVersion>

# Update all packages (use with caution)
dotnet outdated --upgrade
```

---

## Additional Security Recommendations

### Future Enhancements

1. **Content Security Policy (CSP)**
   ```csharp
   app.Use(async (context, next) =>
   {
       context.Response.Headers.Add("Content-Security-Policy", 
           "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';");
       await next();
   });
   ```

2. **X-Frame-Options** (Clickjacking protection)
   ```csharp
   context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
   ```

3. **X-Content-Type-Options**
   ```csharp
   context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
   ```

4. **Referrer Policy**
   ```csharp
   context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
   ```

5. **Rate Limiting** (Prevent brute force attacks)
   - Consider AspNetCoreRateLimit package
   - Limit login attempts per IP
   - Limit API requests per user

6. **Two-Factor Authentication (2FA)**
   - ASP.NET Core Identity supports 2FA
   - Consider authenticator apps or SMS

### Security Audit Schedule

| Task | Frequency | Owner |
|------|-----------|-------|
| Dependency vulnerability scan | Weekly | Dev Team |
| Update security patches | Monthly | Dev Team |
| Review authorization rules | Quarterly | Security Team |
| Penetration testing | Annually | External Auditor |
| SSL certificate renewal | Before expiry | DevOps |

---

## References

- [ASP.NET Core Security Overview](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [HSTS Preload List](https://hstspreload.org/)
- [Content Security Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP)
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)

---

**Document Owner:** Development Team  
**Next Review:** March 2026 (Quarterly)  
**Status:** ✅ Phase 7.2 Complete
