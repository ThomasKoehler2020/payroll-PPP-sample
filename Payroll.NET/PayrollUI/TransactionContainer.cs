using System.Collections;
using Payroll;

namespace PayrollUI
{
    public class TransactionContainer
    {
        public delegate void AddAction();

        private readonly AddAction addAction;

        public IList Transactions { get; } = new ArrayList();

        public TransactionContainer(AddAction action)
        {
            addAction = action;
        }

        public void Add(Transaction transaction)
        {
            Transactions.Add(transaction);
            if (addAction != null)
            {
                addAction();
            }
        }

        public void Clear()
        {
            Transactions.Clear();
        }
    }
}