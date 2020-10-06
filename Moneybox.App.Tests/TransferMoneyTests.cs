namespace Moneybox.App.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moneybox.App.DataAccess;
    using Moneybox.App.Domain;
    using Moneybox.App.Domain.Services;
    using Moneybox.App.Features;
    using Moq;
    using System;

    [TestClass]
    public class TransferMoneyTests
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
        public void TransferMoneyExecute_ExecutingTransfer_RunsWithSuccess()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = 600m;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var toAccount = new Account();

            toAccount.Balance = 20m;
            toAccount.Withdrawn = 20m;
            toAccount.User = new User();
            toAccount.Id = toAccountId;

            var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(toAccountId))))
                .Returns(toAccount);


            var myTransforMoney = new TransferMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myTransforMoney.Execute(fromAccountId, toAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);
            this.mockAccountRepository.Verify(x => x.GetAccountById(toAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);
            this.mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Never);

            this.mockAccountRepository.Verify(x => x.Update(fromAccount), Times.Once);
            this.mockAccountRepository.Verify(x => x.Update(toAccount), Times.Once);

        }

        [TestMethod]
        public void TransferMoneyExecute_ExecutingTransfer_RunsWithSuccessWithExpectedAmountInUpdate()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = 600m;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var expectedFinalBalance = 590m;
            var expectedFinalWithdrawn = 490m;

            var toAccount = new Account();

            toAccount.Balance = 20m;
            toAccount.Withdrawn = 20m;
            toAccount.User = new User();
            toAccount.Id = toAccountId;

            var toExpectedFinalBalance = 30m;
            var toExpectedFinalPaidIn = 10m;

            var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(toAccountId))))
                .Returns(toAccount);


            var myTransforMoney = new TransferMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myTransforMoney.Execute(fromAccountId, toAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);
            this.mockAccountRepository.Verify(x => x.GetAccountById(toAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);
            this.mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Never);

            this.mockAccountRepository.Verify(x => x.Update(It.Is<Account>(
                x => x.Balance == expectedFinalBalance
             && x.Withdrawn == expectedFinalWithdrawn)), Times.Once);
            
            this.mockAccountRepository.Verify(x => x.Update(It.Is<Account>(
                x => x.Balance == toExpectedFinalBalance
             && x.PaidIn == toExpectedFinalPaidIn)), Times.Once);

        }

        [TestMethod]
        public void TransferMoneyExecute_ExecutingTransfer_NotifyFundsLow()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = 500m;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var toAccount = new Account();

            toAccount.Balance = 20m;
            toAccount.Withdrawn = 20m;
            toAccount.User = new User();
            toAccount.Id = toAccountId;

            var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(toAccountId))))
                .Returns(toAccount);


            var myTransforMoney = new TransferMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myTransforMoney.Execute(fromAccountId, toAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);
            this.mockAccountRepository.Verify(x => x.GetAccountById(toAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);
            this.mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Once);

            this.mockAccountRepository.Verify(x => x.Update(fromAccount), Times.Once);
            this.mockAccountRepository.Verify(x => x.Update(toAccount), Times.Once);

        }

        [TestMethod]
        public void TransferMoneyExecute_ExecutingTransfer_NotifyApproachingPayInLimit()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = 700m;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var toAccount = new Account();

            toAccount.Balance = 20m;
            toAccount.Withdrawn = 20m;
            toAccount.User = new User();
            toAccount.Id = toAccountId;
            toAccount.PaidIn = 3600m;

            var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(toAccountId))))
                .Returns(toAccount);


            var myTransforMoney = new TransferMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myTransforMoney.Execute(fromAccountId, toAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);
            this.mockAccountRepository.Verify(x => x.GetAccountById(toAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Once);
            this.mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Never);

            this.mockAccountRepository.Verify(x => x.Update(fromAccount), Times.Once);
            this.mockAccountRepository.Verify(x => x.Update(toAccount), Times.Once);

        }

        [ExpectedException(typeof(InvalidOperationException), "Insufficient funds to make transfer")]
        [TestMethod]
        public void TransferMoneyExecute_ExecutingTransfer_InsufficientFunds()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = -1;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var toAccount = new Account();

            toAccount.Balance = 20m;
            toAccount.Withdrawn = 20m;
            toAccount.User = new User();
            toAccount.Id = toAccountId;

            var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(toAccountId))))
                .Returns(toAccount);


            var myTransforMoney = new TransferMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myTransforMoney.Execute(fromAccountId, toAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);
            this.mockAccountRepository.Verify(x => x.GetAccountById(toAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);
            this.mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Never);

            this.mockAccountRepository.Verify(x => x.Update(fromAccount), Times.Never);
            this.mockAccountRepository.Verify(x => x.Update(toAccount), Times.Never);

        }

        [ExpectedException(typeof(InvalidOperationException), "Account pay in limit reached")]
        [TestMethod]
        public void TransferMoneyExecute_ExecutingTransfer_AccountPayLimit()
        {
            // Arrange

            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account();

            fromAccount.Balance = 600;
            fromAccount.Withdrawn = 500m;
            fromAccount.User = new User();
            fromAccount.Id = fromAccountId;

            var toAccount = new Account();

            toAccount.Balance = 20m;
            toAccount.Withdrawn = 20m;
            toAccount.User = new User();
            toAccount.Id = toAccountId;
            toAccount.PaidIn = 5000m;

            var amount = 10;

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(fromAccountId))))
                .Returns(fromAccount);

            this.mockAccountRepository.Setup(x => x.GetAccountById(It.Is<Guid>(v => v.Equals(toAccountId))))
                .Returns(toAccount);


            var myTransforMoney = new TransferMoney(this.mockAccountRepository.Object, this.mockNotificationService.Object);

            // Act 

            myTransforMoney.Execute(fromAccountId, toAccountId, amount);

            // Assert

            this.mockAccountRepository.Verify(x => x.GetAccountById(fromAccountId), Times.Once);
            this.mockAccountRepository.Verify(x => x.GetAccountById(toAccountId), Times.Once);

            this.mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Once);
            this.mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Never);

            this.mockAccountRepository.Verify(x => x.Update(fromAccount), Times.Never);
            this.mockAccountRepository.Verify(x => x.Update(toAccount), Times.Never);

        }

    }
}
