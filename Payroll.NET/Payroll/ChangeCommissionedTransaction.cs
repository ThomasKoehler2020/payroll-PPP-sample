namespace Payroll
{
    public class ChangeCommissionedTransaction
        : ChangeClassificationTransaction
    {
        private readonly double baseSalary;
        private readonly double commissionRate;

        protected override PaymentClassification Classification => new CommissionClassification(baseSalary, commissionRate);

        protected override PaymentSchedule Schedule => new BiWeeklySchedule();

        public ChangeCommissionedTransaction(int id, double baseSalary, double commissionRate, PayrollDatabase database)
            : base(id, database)
        {
            this.baseSalary = baseSalary;
            this.commissionRate = commissionRate;
        }
    }
}