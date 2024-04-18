using FluentValidation;

namespace StudentEnrollment.Api.DTOs.Authentication
{
    public class LoginDto
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Email invalid");
            
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(20).WithMessage("Password must be less than 20 characters");
        }
    }
}
