using Bank.Services.Commands;
using BankApi.Services.Dto.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Bank.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BankController(IMediator mediator)
        {
            _mediator = mediator;
        }
      
        [SwaggerOperation(Summary = "Create new bank account. Username is required and the Initial Balance must be greater than zero. Also, cannot accept already existing account")]
        [HttpPost("CreateNewAccount")]
        public async Task<IActionResult> CreateNewAccount([FromBody] CreateAccountRequest request)
        {
            var response = await _mediator.Send(new CreateAccount(request)).ConfigureAwait(false);
            return Ok(response);
        }

        [SwaggerOperation(Summary = "Transfer funds to existing account. All fields are required, transfer amount must be greater than Zero and the source account should have balance")]
        [HttpPost("TransferFunds")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundRequest request)
        {
            var response = await _mediator.Send(new TransferFund(request)).ConfigureAwait(false);
            return Ok(response);
        }

    }
}
