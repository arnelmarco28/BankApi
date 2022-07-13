using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bank.Services
{
    /// <summary>
    /// MediatR pipeline that will handle all the validations 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {

            if (_validators.Any())
            {
                var context = new ValidationContext(request);
                var failures = _validators
                    .Select(v => v.Validate(context))
                    .SelectMany(result => result.Errors.Select(w => w))
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }
            return next();
        }
    }
}
