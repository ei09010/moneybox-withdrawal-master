using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public TransferMoney(
            IAccountRepository accountRepository,
            INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = this.accountRepository.GetAccountById(fromAccountId);
            var to = this.accountRepository.GetAccountById(toAccountId);

            // validating account amounts
            
            from.ValidateBalance(this.notificationService, amount);

            to.ValidatePaidIn(this.notificationService, amount);


            // updating account balances, withdrawn and paindIn properties

            from.UpdateBalanceAndWithdrawn(amount);

            to.UpdateBalanceAndPaidIn(amount);


            this.accountRepository.Update(from);
            this.accountRepository.Update(to);
        }
    }
}
