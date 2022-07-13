using Bank.Services.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BankApi.Services.Validators;

namespace Bank.Services
{
    public static class BankService
    {
        public static IServiceCollection AddBankServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            BankValidators(services);            
            return services;
        }
        
        private static void BankValidators(IServiceCollection services)
        {            
            services.AddTransient<IValidator<int>, IdValidator>();      
            services.AddTransient<IValidator<CreateAccount>, CreateAccountValidator>();
            services.AddTransient<IValidator<TransferFund>,TransferFundValidator>();            
        }     
    }
}
