using Bank.Services.Commands;
using FluentValidation;

namespace BankApi.Services.Validators
{
    public class TransferFundValidator : AbstractValidator<TransferFund>
    {
        public TransferFundValidator()
        {
            RuleFor(s => s.TransferFundRequest.SourceAccountId).SetValidator(new IdValidator()).NotEqual(d=>d.TransferFundRequest.DestinationAccountId);
            RuleFor(s => s.TransferFundRequest.DestinationAccountId).SetValidator(new IdValidator()).NotEqual(s => s.TransferFundRequest.SourceAccountId);
            RuleFor(s => s.TransferFundRequest.TransferAmount).GreaterThan(0);
        }
    }
}
