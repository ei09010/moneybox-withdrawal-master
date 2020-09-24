using System;
using System.Collections.Generic;
using System.Text;

namespace Moneybox.App.Domain
{
    public interface ICashValidator
    {
        bool IsValidAmount(int amount);

        bool IsValidBalance(int balance);

    }
}
