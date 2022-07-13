using AutoMapper;
using Bank.DAL.Context;
using Bank.DAL.Entities;
using Bank.Services;
using Bank.Services.Commands;
using BankApi.Services.Dto.Request;
using BankApi.Services.Dto.Response;
using BankApi.Services.Validators;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bank.UnitTest
{
    /// <summary>
    /// Sample unit test 
    /// </summary>
    public class BankUnitTest
    {
        private readonly IBankFactory _bankFactory;
        private readonly CreateAccountAsync _createAccountAsync;
        private readonly TransferFundAsync _transferFundAsync;

        public BankUnitTest()
        {
            _bankFactory = Substitute.For<IBankFactory>();

            Mapper.Reset();
            Mapper.Initialize(cfg => { cfg.AddProfile(new ServiceMapperProfile()); });
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceMapperProfile>());
            var _mapper = config.CreateMapper();
            _createAccountAsync = new CreateAccountAsync(_bankFactory, _mapper);
            var transferLogger = Substitute.For<ILogger<TransferFundAsync>>();
            _transferFundAsync = new TransferFundAsync(_bankFactory, _mapper, transferLogger);
            SetupMocking();
        }

        private void SetupMocking()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _bankFactory.Create().Returns(new BankContext(options));

            using (var context = new BankContext(options))
            {
                var accountFirst = new Account()
                {
                    AccountID = 1,
                    Username = "test account",
                    InitialBalance = 100m
                };

                var accountSecond = new Account()
                {
                    AccountID = 2,
                    Username = "test account 2",
                    InitialBalance = 100m
                };

                context.Account.Add(accountFirst);
                context.Account.Add(accountSecond);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task CreateAccountAllParametersValid()
        {
            //arange
            var newAccount = new CreateAccountRequest()
            {
                Username = "Unique Account",
                InitialBalance = 100
            };

            var request = new CreateAccount(newAccount);
            var response = new CommonResponse<CreateAccountResponse>() { Data = null, IsSuccess = false };
            var validator = new CreateAccountValidator();

            //act
            //manual call to  validation was handle by mediatr pipeline
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                response = await _createAccountAsync.Handle(request, default(CancellationToken));
            }

            //assert
            validationResult.IsValid.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data.AccountID.Should().NotBe(0);
        }

        [Fact]
        public async Task CreateAccountInvalidInitialBalance()
        {
            //arange
            var newAccount = new CreateAccountRequest()
            {
                Username = "Unique Account",
                InitialBalance = 0
            };

            var request = new CreateAccount(newAccount);
            var response = new CommonResponse<CreateAccountResponse>() { Data = null, IsSuccess = false };
            var validator = new CreateAccountValidator();

            //act            
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                response = await _createAccountAsync.Handle(request, default(CancellationToken));
            }

            //assert
            validationResult.IsValid.Should().BeFalse();
            response.Data.Should().BeNull();
        }

        [Fact]
        public async Task CreateAlreadyExist()
        {
            //arange
            var newAccount = new CreateAccountRequest()
            {
                Username = "test account",
                InitialBalance = 100
            };

            var request = new CreateAccount(newAccount);
            var response = new CommonResponse<CreateAccountResponse>() { Data = null, IsSuccess = false };
            var validator = new CreateAccountValidator();

            //act            
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                response = await _createAccountAsync.Handle(request, default(CancellationToken));
            }

            //assert
            validationResult.IsValid.Should().BeTrue();
            response.Data.Should().BeNull();
            response.ErrorMesssage.Should().BeEquivalentTo("Username Already Exist!");
        }

        [Fact]
        public async Task TransferAllParametersValid()
        {
            //arange
            var newTransfer = new TransferFundRequest()
            {
                DestinationAccountId = 1,
                SourceAccountId = 2,
                TransferAmount = 50
            };

            var request = new TransferFund(newTransfer);
            var response = new CommonResponse<TransferFundResponse>() { Data = null, IsSuccess = false };
            var validator = new TransferFundValidator();

            //act            
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                response = await _transferFundAsync.Handle(request, default(CancellationToken));
            }

            //assert
            validationResult.IsValid.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data.TransferID.Should().NotBe(0);
        }

        [Fact]
        public async Task TransferWithInValidSourceAccount()
        {
            //arange
            var newTransfer = new TransferFundRequest()
            {
                DestinationAccountId = 1,
                SourceAccountId = 33,
                TransferAmount = 50
            };

            var request = new TransferFund(newTransfer);
            var response = new CommonResponse<TransferFundResponse>() { Data = null, IsSuccess = false };
            var validator = new TransferFundValidator();

            //act            
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                response = await _transferFundAsync.Handle(request, default(CancellationToken));
            }

            //assert
            validationResult.IsValid.Should().BeTrue();
            response.Data.Should().BeNull();
            response.ErrorMesssage.Should().BeEquivalentTo("SourceAccountId Not Exist!");
        }

        [Fact]
        public async Task TransferWithInValidDestinationAccount()
        {
            //arange
            var newTransfer = new TransferFundRequest()
            {
                DestinationAccountId = 22,
                SourceAccountId = 1,
                TransferAmount = 50
            };

            var request = new TransferFund(newTransfer);
            var response = new CommonResponse<TransferFundResponse>() { Data = null, IsSuccess = false };
            var validator = new TransferFundValidator();

            //act            
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                response = await _transferFundAsync.Handle(request, default(CancellationToken));
            }

            //assert
            validationResult.IsValid.Should().BeTrue();
            response.Data.Should().BeNull();
            response.ErrorMesssage.Should().BeEquivalentTo("DestinationAccountId Not Exist!");
        }

        [Fact]
        public async Task TransferWillZeroTheSourceBalance()
        {
            //arange
            var newTransfer = new TransferFundRequest()
            {
                DestinationAccountId = 2,
                SourceAccountId = 1,
                TransferAmount = 100
            };

            var request = new TransferFund(newTransfer);
            var response = new CommonResponse<TransferFundResponse>() { Data = null, IsSuccess = false };
            var validator = new TransferFundValidator();

            //act            
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                response = await _transferFundAsync.Handle(request, default(CancellationToken));
            }

            //assert
            validationResult.IsValid.Should().BeTrue();
            response.Data.Should().BeNull();
            response.ErrorMesssage.Should().BeEquivalentTo("Not Enough Funds!");
        }
    }
}
