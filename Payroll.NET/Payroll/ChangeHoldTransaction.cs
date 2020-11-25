namespace Payroll
{
    public class ChangeHoldTransaction : ChangeMethodTransaction
    {
        protected override PaymentMethod Method => new HoldMethod();

        public ChangeHoldTransaction(int empId, PayrollDatabase database)
            : base(empId, database)
        {
        }
    }
}