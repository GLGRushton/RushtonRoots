using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Application.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RushtonRootsDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RushtonRootsDbContext context,
        IEmailService emailService,
        ILogger<AccountService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<bool> LoginAsync(LoginViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            _logger.LogWarning("Login attempt for non-existent user: {Email}", model.Email);
            return false;
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in successfully", model.Email);
            return true;
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {Email} account locked out", model.Email);
        }
        else
        {
            _logger.LogWarning("Invalid login attempt for user: {Email}", model.Email);
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            _logger.LogInformation("Password reset requested for non-existent account");
            return true;
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendPasswordResetEmailAsync(user.Email!, code);
        
        _logger.LogInformation("Password reset email sent to user account");
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            _logger.LogWarning("Password reset attempted for non-existent user: {Email}", model.Email);
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("Password reset successful for user: {Email}", model.Email);
            return true;
        }

        _logger.LogWarning("Password reset failed for user {Email}: {Errors}", 
            model.Email, 
            string.Join(", ", result.Errors.Select(e => e.Description)));
        return false;
    }

    public async Task<UserProfileViewModel?> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        var person = user.PersonId.HasValue 
            ? await _context.People.FindAsync(user.PersonId.Value)
            : null;

        return new UserProfileViewModel
        {
            Id = user.Id,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            EmailConfirmed = user.EmailConfirmed,
            PersonId = user.PersonId,
            PersonName = person != null ? $"{person.FirstName} {person.LastName}" : null
        };
    }

    public async Task<bool> UpdateUserProfileAsync(string userId, UserProfileViewModel model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.PhoneNumber = model.PhoneNumber;
        
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation("User profile updated for: {Email}", user.Email);
            return true;
        }

        _logger.LogWarning("User profile update failed for {Email}: {Errors}",
            user.Email,
            string.Join(", ", result.Errors.Select(e => e.Description)));
        return false;
    }

    public async Task<bool> CreateUserAsync(CreateUserRequest request, string createdByUserId)
    {
        // Verify the creator has permission
        var person = await _context.People.FindAsync(request.PersonId);
        if (person == null)
        {
            _logger.LogWarning("Attempted to create user for non-existent person: {PersonId}", request.PersonId);
            return false;
        }

        var creatorUser = await _userManager.FindByIdAsync(createdByUserId);
        if (creatorUser == null || !creatorUser.PersonId.HasValue)
        {
            _logger.LogWarning("Invalid creator user: {UserId}", createdByUserId);
            return false;
        }

        // Check if creator has permission to manage this household
        var canManage = await CanUserManageHousehold(createdByUserId, person.HouseholdId);
        if (!canManage)
        {
            _logger.LogWarning("User {UserId} does not have permission to create users in household {HouseholdId}",
                createdByUserId, person.HouseholdId);
            return false;
        }

        // Create the user
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            PersonId = request.PersonId
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            _logger.LogWarning("User creation failed for {Email}: {Errors}",
                request.Email,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }

        // Assign role if specified
        if (!string.IsNullOrEmpty(request.Role))
        {
            await _userManager.AddToRoleAsync(user, request.Role);
        }

        // Generate email confirmation token and send email
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailConfirmationAsync(user.Email!, code);

        _logger.LogInformation("User created successfully for {Email} by {CreatorId}", request.Email, createdByUserId);
        return true;
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            _logger.LogInformation("Email confirmed for user: {Email}", user.Email);
            return true;
        }

        _logger.LogWarning("Email confirmation failed for user {Email}: {Errors}",
            user.Email,
            string.Join(", ", result.Errors.Select(e => e.Description)));
        return false;
    }

    public async Task<bool> ResendEmailConfirmationAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.EmailConfirmed)
        {
            return false;
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailConfirmationAsync(user.Email!, code);

        _logger.LogInformation("Email confirmation resent to: {Email}", email);
        return true;
    }

    public async Task<bool> CanUserManageHousehold(string userId, int householdId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !user.PersonId.HasValue)
        {
            return false;
        }

        // Check if user is in Admin role (system-wide admin)
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return true;
        }

        // Check if user has ADMIN permission in this household
        var permission = await _context.HouseholdPermissions
            .FirstOrDefaultAsync(hp => 
                hp.PersonId == user.PersonId.Value && 
                hp.HouseholdId == householdId && 
                hp.Role == "ADMIN");

        return permission != null;
    }
}
