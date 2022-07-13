
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Bank.DAL.Context;
using System.IO;

namespace Bank.WebApi
{
    public class BankContextFactory : IDesignTimeDbContextFactory<BankContext>
    {      
        public BankContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

            var connectionString = configuration.GetConnectionString("BankEntities");

            var optionBuilder = new DbContextOptionsBuilder<BankContext>();
            
            optionBuilder.UseSqlServer(connectionString);
            return new BankContext(optionBuilder.Options);
        }
    }
}
