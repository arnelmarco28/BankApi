namespace BankApi.Services.Dto.Request
{
    public class CreateAccountRequest
    {
        public string Username { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
