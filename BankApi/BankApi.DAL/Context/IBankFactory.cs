using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.DAL.Context
{
    public interface IBankFactory
    {
        BankContext Create();
    }
}
