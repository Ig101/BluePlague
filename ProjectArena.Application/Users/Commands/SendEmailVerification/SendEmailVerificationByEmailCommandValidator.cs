using System;
using FluentValidation;

namespace ProjectArena.Application.Users.Commands.SendEmailVerification
{
  public class SendEmailVerificationByEmailCommandValidator : AbstractValidator<SendEmailVerificationByEmailCommand>
  {
    public SendEmailVerificationByEmailCommandValidator()
    {
      RuleFor(x => x.Email).NotEmpty();
    }
  }
}