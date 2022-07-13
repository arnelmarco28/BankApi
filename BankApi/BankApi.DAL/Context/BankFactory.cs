using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.DAL.Context
{
    public class BankFactory : IBankFactory
    {
        private readonly string _connectionString;

        public BankFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BankContext Create()
        {
            return new BankContext(_connectionString);
        }      
      
    }
}
