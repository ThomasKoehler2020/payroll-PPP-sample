using System;
using NUnit.Framework;
using Payroll;

namespace PayrollUI
{
    [TestFixture]
    public class PayrollPresenterTest
    {
        [SetUp]
        public void SetUp()
        {
            view = new MockPayrollView();
            database = new InMemoryPayrollDatabase();
            database.Clear();
            viewLoader = new MockViewLoader();
            presenter = new PayrollPresenter(database, viewLoader);
            presenter.View = view;
        }

        private MockPayrollView view;
        private PayrollPresenter presenter;
        private InMemoryPayrollDatabase database;
        private MockViewLoader viewLoader;

        [Test]
        public void AddAction()
        {
            var container =
                presenter.TransactionContainer;
            Transaction transaction = new MockTransaction();

            container.Add(transaction);

            var expected = transaction
                           + Environment.NewLine;
            Assert.AreEqual(expected, view.transactionsText);
        }

        [Test]
        public void AddEmployeeAction()
        {
            presenter.AddEmployeeActionInvoked();

            Assert.IsTrue(viewLoader.addEmployeeViewWasLoaded);
        }

        [Test]
        public void Creation()
        {
            Assert.AreSame(view, presenter.View);
            Assert.AreSame(database, presenter.Database);
            Assert.IsNotNull(presenter.TransactionContainer);
        }

        [Test]
        public void RunTransactions()
        {
            var transaction = new MockTransaction();
            presenter.TransactionContainer.Add(transaction);
            var employee =
                new Employee(123, "John", "123 Baker St.");
            database.AddEmployee(employee);

            presenter.RunTransactions();

            Assert.IsTrue(transaction.wasExecuted);
            Assert.AreEqual("", view.transactionsText);
            var expectedEmployeeTest = employee
                                       + Environment.NewLine;
            Assert.AreEqual(expectedEmployeeTest, view.employeesText);
        }
    }
}