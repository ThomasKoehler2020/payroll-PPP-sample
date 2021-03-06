using System.Windows.Forms;
using Payroll;

namespace PayrollUI
{
    public class PayrollMain
    {
        public static void Main(string[] args)
        {
            PayrollDatabase database =
                new InMemoryPayrollDatabase();
            var viewLoader =
                new WindowViewLoader(database);

            viewLoader.LoadPayrollView();
            Application.Run(viewLoader.LastLoadedView);
        }
    }
}