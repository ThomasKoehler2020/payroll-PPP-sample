using System.Windows.Forms;
using Payroll;

namespace PayrollUI
{
    public class WindowViewLoader : ViewLoader
    {
        private readonly PayrollDatabase database;

        public Form LastLoadedView { get; private set; }

        public WindowViewLoader(PayrollDatabase database)
        {
            this.database = database;
        }

        public void LoadPayrollView()
        {
            var view = new PayrollWindow();
            var presenter =
                new PayrollPresenter(database, this);

            view.Presenter = presenter;
            presenter.View = view;

            LoadView(view);
        }

        public void LoadAddEmployeeView(
            TransactionContainer transactionContainer)
        {
            var view = new AddEmployeeWindow();
            var presenter =
                new AddEmployeePresenter(view,
                    transactionContainer, database);

            view.Presenter = presenter;

            LoadView(view);
        }

        private void LoadView(Form view)
        {
            view.Show();
            LastLoadedView = view;
        }
    }
}