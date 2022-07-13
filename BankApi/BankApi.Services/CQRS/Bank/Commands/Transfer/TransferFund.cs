using BankApi.Services.Dto.Request;
using BankApi.Services.Dto.Response;
using MediatR;

namespace Bank.Services.Commands
{
    public class TransferFund : IRequest<CommonResponse<TransferFundResponse>>
    {
        public TransferFundRequest TransferFundRequest { get; set; }

        public TransferFund(TransferFundRequest transferFundRequest)
        {
            TransferFundRequest = transferFundRequest;
        }
    }
}
