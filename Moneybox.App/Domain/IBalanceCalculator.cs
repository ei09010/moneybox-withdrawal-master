using System;
using System.Collections.Generic;
using System.Text;

namespace Moneybox.App.Domain
{
    public interface IBalanceCalculator
    {
        Account UpdateBalance(decimal amount);

        Account MoveMoney(decimal amount);
    }
}
