using BankApi.Services.Dto.Request;
using BankApi.Services.Dto.Response;
using MediatR;

namespace Bank.Services.Commands
{
    public class CreateAccount : IRequest<CommonResponse<CreateAccountResponse>>
    {
        public CreateAccountRequest CreateAccountRequest { get; set; }

        public CreateAccount(CreateAccountRequest createAccountRequest)
        {
            CreateAccountRequest = createAccountRequest;
        }
    }
}
