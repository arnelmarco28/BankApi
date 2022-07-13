using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApi.Services.Validators
{
    public class IdValidator : AbstractValidator<int>
    {
        public IdValidator()
        {
            RuleFor(s => s).NotNull().WithMessage("ID should be not null")
              .NotEmpty().WithMessage("ID should be not empty")
              .Must(x => x > 0).WithMessage("ID should be numeric");
        }

    }
}
