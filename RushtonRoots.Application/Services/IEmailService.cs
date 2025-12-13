namespace RushtonRoots.Application.Services;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(string email, string confirmationToken);
    Task SendPasswordResetEmailAsync(string email, string resetToken);
}
