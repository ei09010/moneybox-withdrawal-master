using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App
{
    public class Account : ICashValidation, ICashCalculation
    {
        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public void ValidateBalance(INotificationService notificationService, decimal amount)
        {
            var fromBalance = this.Balance - amount;

            if (fromBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            if (fromBalance < 500m)
            {
                notificationService.NotifyFundsLow(this.User.Email);
            }
        }

        public void ValidatePaidIn(INotificationService notificationService, decimal amount)
        {
            var paidIn = this.PaidIn + amount;

            if (paidIn > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (PayInLimit - paidIn < 500m)
            {
                notificationService.NotifyApproachingPayInLimit(this.User.Email);
            }
        }

        public void UpdateBalanceAndWithdrawn(decimal amount)
        {
            this.Balance -= amount;
            this.Withdrawn -= amount;
        }

        public void UpdateBalanceAndPaidIn(decimal amount)
        {
            this.Balance += amount;
            this.PaidIn += amount;
        }
    }
}
