using AutoMapper;
using Bank.DAL.Context;
using Bank.DAL.Entities;
using BankApi.Services.Dto.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Bank.Services.Commands
{
    public class CreateAccountAsync : IRequestHandler<CreateAccount, CommonResponse<CreateAccountResponse>>
    {
        private readonly IBankFactory _bankFactory;
        private readonly IMapper _mapper;

        public CreateAccountAsync(IBankFactory bankFactory, IMapper mapper)
        {
            _bankFactory = bankFactory;
            _mapper = mapper;
        }
        public async Task<CommonResponse<CreateAccountResponse>> Handle(CreateAccount request, CancellationToken cancellationToken)
        {
            // not needed try catch block here it will be handle by global error handler and the validation and has error logging
            using (var context = _bankFactory.Create())
            {               
                var newAccount = _mapper.Map<Account>(request.CreateAccountRequest);
                var isExist = await context.Account.AsNoTracking().AnyAsync(x => x.Username.ToUpper() == request.CreateAccountRequest.Username.ToUpper()).ConfigureAwait(false);
                // return Username Already Exist!
                if (isExist) 
                {                    
                    return new CommonResponse<CreateAccountResponse>() { Data = null, IsSuccess = false, ErrorMesssage = "Username Already Exist!" };                    
                }
                // create account if username is unique and return the accountID
                context.Account.Add(newAccount);
                await context.SaveChangesAsync();
                return new CommonResponse<CreateAccountResponse>() { Data = new CreateAccountResponse { AccountID = newAccount.AccountID }, IsSuccess = true };
            }
        }
    }
}
