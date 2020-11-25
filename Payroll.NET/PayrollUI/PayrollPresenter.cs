using System;
using System.Text;
using Payroll;

namespace PayrollUI
{
    public class PayrollPresenter
    {
        private readonly ViewLoader viewLoader;

        public PayrollView View { get; set; }

        public TransactionContainer TransactionContainer { get; }

        public PayrollDatabase Database { get; }

        public PayrollPresenter(PayrollDatabase database,
            ViewLoader viewLoader)
        {
            View = View;
            Database = database;
            this.viewLoader = viewLoader;
            TransactionContainer.AddAction addAction =
                TransactionAdded;
            TransactionContainer = new TransactionContainer(addAction);
        }

        public void TransactionAdded()
        {
            UpdateTransactionsTextBox();
        }

        public virtual void AddEmployeeActionInvoked()
        {
            viewLoader.LoadAddEmployeeView(TransactionContainer);
        }

        public virtual void RunTransactions()
        {
            foreach (Transaction transaction in
                TransactionContainer.Transactions)
            {
                transaction.Execute();
            }

            TransactionContainer.Clear();
            UpdateTransactionsTextBox();
            UpdateEmployeesTextBox();
        }

        private void UpdateTransactionsTextBox()
        {
            var builder = new StringBuilder();
            foreach (Transaction transaction in
                TransactionContainer.Transactions)
            {
                builder.Append(transaction);
                builder.Append(Environment.NewLine);
            }

            View.TransactionsText = builder.ToString();
        }

        private void UpdateEmployeesTextBox()
        {
            var builder = new StringBuilder();
            foreach (Employee employee in Database.GetAllEmployees())
            {
                builder.Append(employee);
                builder.Append(Environment.NewLine);
            }

            View.EmployeesText = builder.ToString();
        }
    }
}