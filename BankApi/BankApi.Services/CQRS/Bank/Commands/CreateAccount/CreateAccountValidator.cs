using Bank.Services.Commands;
using FluentValidation;

namespace BankApi.Services.Validators
{
    public class CreateAccountValidator : AbstractValidator<CreateAccount>
    {
        public CreateAccountValidator()
        {
            RuleFor(s => s.CreateAccountRequest.Username).NotNull().NotEmpty();
            RuleFor(s => s.CreateAccountRequest.InitialBalance).GreaterThan(0);                               
        }
    }
}
