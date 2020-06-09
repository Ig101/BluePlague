using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Domain.Identity;
using ProjectArena.Infrastructure.Models.ErrorHandling;

namespace ProjectArena.Application.Users.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest
    {
        public string UserId { get; set; }

        public string Code { get; set; }

        public string Password { get; set; }

        private class Handler : IRequestHandler<ChangePasswordCommand>
        {
            private readonly IdentityUserManager _userManager;

            public Handler(IdentityUserManager userManager)
            {
                _userManager = userManager;
            }

            public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    throw new ValidationErrorsException(new[]
                    {
                        new HttpErrorInfo()
                        {
                            Key = "id",
                            Description = "User is not found."
                        }
                    });
                }

                var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
                if (!result.Succeeded)
                {
                    throw new ValidationErrorsException(result.Errors.Select(x => new HttpErrorInfo()
                    {
                        Key = x.Code,
                        Description = x.Description
                    }));
                }

                return Unit.Value;
            }
        }
    }
}