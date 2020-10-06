namespace Moneybox.App.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moneybox.App.DataAccess;
    using Moneybox.App.Domain.Services;
    using Moneybox.App.Features;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [TestClass]
    public class WithdrawMoneyTests
    {

        private Mock<IAccountRepository> mockAccountRepository;
        private Mock<INotificationService> mockNotificationService;

        [TestInitialize]
        public void TestInit()
        {
            this.mockAccountRepository = new Mock<IAccountRepository>();
            this.mockNotificationService = new Mock<INotificationService>();
        }

        [TestMethod]
        public void WithdrawMoneyExecute_ExecutingWithdraw_RunsWithSuccess()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = 600m;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);


            var myWithdrawMoeny = new WithdrawMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myWithdrawMoeny.Execute(fromAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);
            this.mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Never);

            this.mockAccountRepository.Verify(x => x.Update(fromAccount), Times.Once);
        }

        [TestMethod]
        public void WithdrawMoneyExecute_ExecutingWithdraw_RunsWithSuccessWithExpectedAmountInUpdate()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = 600m;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var expectedFinalBalance = 590m;
            var expectedFinalWithdrawn = 490m;

           var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);


            var myWithdrawMoney = new WithdrawMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myWithdrawMoney.Execute(fromAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);
            this.mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Never);

            this.mockAccountRepository.Verify(x => x.Update(It.Is<Account>(
                x => x.Balance == expectedFinalBalance
             && x.Withdrawn == expectedFinalWithdrawn)), Times.Once);

        }

        [TestMethod]
        public void WithdrawMoneyExecute_ExecutingWithdraw_NotifyFundsLow()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = 500m;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);

            var myWithdrawMoney = new WithdrawMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myWithdrawMoney.Execute(fromAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);
            this.mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Once);

            this.mockAccountRepository.Verify(x => x.Update(fromAccount), Times.Once);

        }

        [ExpectedException(typeof(InvalidOperationException), "Insufficient funds to make transfer")]
        [TestMethod]
        public void WithdrawMoneyExecute_ExecutingWithdraw_InsufficientFunds()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = -1;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);


            var myWithdrawMoney = new WithdrawMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myWithdrawMoney.Execute(fromAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);

            this.mockAccountRepository.Verify(x => x.Update(fromAccount), Times.Never);

        }

    }
}
