using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IAccountService
{
    Task<bool> LoginAsync(LoginViewModel model);
    Task LogoutAsync();
    Task<bool> ForgotPasswordAsync(ForgotPasswordViewModel model);
    Task<bool> ResetPasswordAsync(ResetPasswordViewModel model);
    Task<UserProfileViewModel?> GetUserProfileAsync(string userId);
    Task<bool> UpdateUserProfileAsync(string userId, UserProfileViewModel model);
    Task<bool> CreateUserAsync(CreateUserRequest request, string createdByUserId);
    Task<bool> ConfirmEmailAsync(string userId, string code);
    Task<bool> ResendEmailConfirmationAsync(string email);
    Task<bool> CanUserManageHousehold(string userId, int householdId);
}
