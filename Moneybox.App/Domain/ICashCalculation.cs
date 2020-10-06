using System;
using System.Collections.Generic;
using System.Text;

namespace Moneybox.App.Domain
{
    public interface ICashCalculation
    {
        void UpdateBalanceAndWithdrawn(decimal amount);

        void UpdateBalanceAndPaidIn(decimal amount);
    }
}
