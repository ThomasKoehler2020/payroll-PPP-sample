using NUnit.Framework;
using Payroll;

namespace PayrollUI
{
    [TestFixture]
    public class WindowViewLoaderTest
    {
        [SetUp]
        public void SetUp()
        {
            database = new InMemoryPayrollDatabase();
            viewLoader = new WindowViewLoader(database);
        }

        private PayrollDatabase database;
        private WindowViewLoader viewLoader;

        [Test]
        public void LoadAddEmployeeView()
        {
            viewLoader.LoadAddEmployeeView(
                new TransactionContainer(null));

            var form = viewLoader.LastLoadedView;
            Assert.IsTrue(form is AddEmployeeWindow);
            Assert.IsTrue(form.Visible);

            var addEmployeeWindow =
                form as AddEmployeeWindow;
            Assert.IsNotNull(addEmployeeWindow.Presenter);
        }

        [Test]
        public void LoadPayrollView()
        {
            viewLoader.LoadPayrollView();

            var form = viewLoader.LastLoadedView;
            Assert.IsTrue(form is PayrollWindow);
            Assert.IsTrue(form.Visible);

            var payrollWindow = form as PayrollWindow;
            var presenter = payrollWindow.Presenter;
            Assert.IsNotNull(presenter);
            Assert.AreSame(form, presenter.View);
        }
    }
}