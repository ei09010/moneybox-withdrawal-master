using Moneybox.App.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moneybox.App.Domain
{
    public interface ICashValidation
    {
        void ValidateBalance(INotificationService notificationService, decimal amount);

        void ValidatePaidIn(INotificationService notificationService, decimal amount);

    }
}
