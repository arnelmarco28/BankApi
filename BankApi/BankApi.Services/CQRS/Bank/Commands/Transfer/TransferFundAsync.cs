using AutoMapper;
using Bank.DAL.Context;
using Bank.DAL.Entities;
using BankApi.Services.Dto.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bank.Services.Commands
{
    public class TransferFundAsync : IRequestHandler<TransferFund, CommonResponse<TransferFundResponse>>
    {
        private readonly IBankFactory _bankFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<TransferFundAsync> _logger;        

        public TransferFundAsync(IBankFactory bankFactory, IMapper mapper, ILogger<TransferFundAsync> logger)
        {
            _bankFactory = bankFactory;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<CommonResponse<TransferFundResponse>> Handle(TransferFund request, CancellationToken cancellationToken)
        {
            // not needed try catch block here it will be handle by global error handler and the validation and has error logging
            using (var context = _bankFactory.Create())
            {
                var newTransFer = _mapper.Map<Transfer>(request.TransferFundRequest);
                //check if the Source Account Exist
                var sourceAccount = await context.Account.FirstOrDefaultAsync(x => x.AccountID == request.TransferFundRequest.SourceAccountId);
                if (sourceAccount == null) 
                {                    
                    return new CommonResponse<TransferFundResponse>() { Data = null, IsSuccess = false, ErrorMesssage = "SourceAccountId Not Exist!" };                    
                }
                //check if the Destination Account Exist
                var destinationAccount = await context.Account.FirstOrDefaultAsync(x => x.AccountID == request.TransferFundRequest.DestinationAccountId);
                if (destinationAccount == null)
                {
                    return new CommonResponse<TransferFundResponse>() { Data = null, IsSuccess = false, ErrorMesssage = "DestinationAccountId Not Exist!" };
                }
                //check if the source has funds Account Exist
                var sourcebalance = sourceAccount.InitialBalance - request.TransferFundRequest.TransferAmount;
                if (sourcebalance > 0)
                {
                    var transaction = await context.Database.BeginTransactionAsync();
                    try
                    {                                                
                            await context.Transfer.AddAsync(newTransFer);                            
                            sourceAccount.InitialBalance = sourcebalance;                            
                            destinationAccount.InitialBalance = destinationAccount.InitialBalance + request.TransferFundRequest.TransferAmount;
                            await context.SaveChangesAsync();                            
                            await transaction.CommitAsync();
                            return new CommonResponse<TransferFundResponse>() { Data = new TransferFundResponse {TransferID= newTransFer.TransferID}, IsSuccess = true };
                        
                    } catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError($"Error in TransferFundAsync: {ex.StackTrace}");
                        return new CommonResponse<TransferFundResponse>() { Data = null, IsSuccess = false, ErrorMesssage = ex.Message };
                    }                    
                }      
                else
                {
                    return new CommonResponse<TransferFundResponse>() { Data = null, IsSuccess = false, ErrorMesssage = "Not Enough Funds!" };
                }                                
            }
        }
    }
}
