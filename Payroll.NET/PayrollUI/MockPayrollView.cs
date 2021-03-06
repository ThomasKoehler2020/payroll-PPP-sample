namespace PayrollUI
{
    public class MockPayrollView : PayrollView
    {
        public string employeesText;
        public PayrollPresenter presenter;
        public string transactionsText;

        public string TransactionsText
        {
            set => transactionsText = value;
        }

        public string EmployeesText
        {
            set => employeesText = value;
        }

        public PayrollPresenter Presenter
        {
            set => presenter = value;
        }
    }
}