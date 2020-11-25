using NUnit.Framework;
using Payroll;

namespace PayrollUI
{
    [TestFixture]
    public class TransactionContainerTest
    {
        [SetUp]
        public void SetUp()
        {
            TransactionContainer.AddAction action =
                SillyAddAction;
            container = new TransactionContainer(action);
            transaction = new MockTransaction();
        }

        private TransactionContainer container;
        private bool addActionCalled;
        private Transaction transaction;

        private void SillyAddAction()
        {
            addActionCalled = true;
        }

        [Test]
        public void AddingTransaction()
        {
            container.Add(transaction);

            var transactions = container.Transactions;
            Assert.AreEqual(1, transactions.Count);
            Assert.AreSame(transaction, transactions[0]);
        }

        [Test]
        public void AddingTransactionTriggersDelegate()
        {
            container.Add(transaction);

            Assert.IsTrue(addActionCalled);
        }

        [Test]
        public void Construction()
        {
            Assert.AreEqual(0, container.Transactions.Count);
        }
    }
}