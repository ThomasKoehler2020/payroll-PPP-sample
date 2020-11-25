namespace Payroll
{
    public class ChangeHourlyTransaction
        : ChangeClassificationTransaction
    {
        private readonly double hourlyRate;

        protected override PaymentClassification Classification => new HourlyClassification(hourlyRate);

        protected override PaymentSchedule Schedule => new WeeklySchedule();

        public ChangeHourlyTransaction(int id, double hourlyRate, PayrollDatabase database)
            : base(id, database)
        {
            this.hourlyRate = hourlyRate;
        }
    }
}