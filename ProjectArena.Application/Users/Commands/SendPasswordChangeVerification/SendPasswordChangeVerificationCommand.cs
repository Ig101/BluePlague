using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectArena.Domain.Email;
using ProjectArena.Domain.Identity;
using ProjectArena.Infrastructure;
using ProjectArena.Infrastructure.Models.Email;
using ProjectArena.Infrastructure.Models.ErrorHandling;

namespace ProjectArena.Application.Users.Commands.SendPasswordChangeVerification
{
    public class SendPasswordChangeVerificationCommand : IRequest
    {
        public string Email { get; set; }

        private class Handler : IRequestHandler<SendPasswordChangeVerificationCommand>
        {
            private readonly IdentityUserManager _userManager;
            private readonly EmailSender _emailSender;
            private readonly ILogger<SendPasswordChangeVerificationCommand> _logger;
            private readonly ServerSettings _serverSettings;

            public Handler(
                IdentityUserManager userManager,
                EmailSender emailSender,
                IOptions<ServerSettings> serverSettings,
                ILogger<SendPasswordChangeVerificationCommand> logger)
            {
                _emailSender = emailSender;
                _logger = logger;
                _serverSettings = serverSettings.Value;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(SendPasswordChangeVerificationCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    throw new ValidationErrorsException(new[]
                    {
                        new HttpErrorInfo()
                        {
                            Key = "email",
                            Description = $"User with email {request.Email} is not found."
                        }
                    });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                try
                {
                    await _emailSender.SendAsync(new EmailMessage()
                    {
                        ToAdresses = new[] { request.Email },
                        Subject = "Change password request",
                        Body = $"<p>Hello!</p><p>If you want to change you password in Blue Plague, follow the link <a href=\"{_serverSettings.Site}/auth/signin/new-password/{user.Id}/{HttpUtility.UrlEncode(token)}\">link</a>.</p><p>If you didn't request this message, just ignore it.</p>"
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Email exception");
                    throw new ServiceUnreachableException("Cannot send email. Try again later...");
                }

                return Unit.Value;
            }
        }
    }
}