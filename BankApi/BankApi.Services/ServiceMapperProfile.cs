using AutoMapper;
using Bank.DAL.Entities;
using BankApi.Services.Dto.Request;

namespace Bank.Services
{
    public class ServiceMapperProfile : Profile
    {
        public ServiceMapperProfile()
        {
            CreateMapForBank();
        }

        private void CreateMapForBank()
        {            
            CreateMap<Account, CreateAccountRequest>().ReverseMap();
            CreateMap<Transfer, TransferFundRequest>().ReverseMap();
        }
    }
}
