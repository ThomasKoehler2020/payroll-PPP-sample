namespace Payroll
{
    public class AddCommissionedEmployee : AddEmployeeTransaction
    {
        private readonly double baseRate;
        private readonly double commissionRate;

        public AddCommissionedEmployee(int id, string name, string address, double baseRate, double commissionRate, PayrollDatabase database)
            : base(id, name, address, database)
        {
            this.baseRate = baseRate;
            this.commissionRate = commissionRate;
        }

        protected override PaymentClassification MakeClassification()
        {
            return new CommissionClassification(baseRate, commissionRate);
        }

        protected override PaymentSchedule MakeSchedule()
        {
            return new BiWeeklySchedule();
        }
    }
}