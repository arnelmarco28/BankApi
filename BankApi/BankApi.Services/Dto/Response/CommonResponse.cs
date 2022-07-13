namespace BankApi.Services.Dto.Response
{
    public class CommonResponse<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMesssage { get; set; }
    }
}
