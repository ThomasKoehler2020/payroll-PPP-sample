namespace Payroll
{
    public class ChangeSalariedTransaction : ChangeClassificationTransaction
    {
        private readonly double salary;

        protected override PaymentClassification Classification => new SalariedClassification(salary);

        protected override PaymentSchedule Schedule => new MonthlySchedule();

        public ChangeSalariedTransaction(int id, double salary, PayrollDatabase database)
            : base(id, database)
        {
            this.salary = salary;
        }
    }
}