using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Domain.Identity;
using ProjectArena.Infrastructure.Models.ErrorHandling;

namespace ProjectArena.Application.Users.Commands.ChangePassword
{
    public class ChangePasswordAuthorizedCommand : IRequest
    {
        public string UserName { get; set; }

        public string CurrentPassword { get; set; }

        public string Password { get; set; }

        private class Handler : IRequestHandler<ChangePasswordAuthorizedCommand>
        {
            private readonly IdentityUserManager _userManager;

            public Handler(IdentityUserManager userManager)
            {
                _userManager = userManager;
            }

            public async Task<Unit> Handle(ChangePasswordAuthorizedCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
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

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.Password);
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