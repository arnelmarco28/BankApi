namespace BankApi.Services.Dto.Request
{
    public class TransferFundRequest
    {
        public int SourceAccountId { get; set; }
        public int DestinationAccountId { get; set; }
        public decimal TransferAmount { get; set; }
    }
}
