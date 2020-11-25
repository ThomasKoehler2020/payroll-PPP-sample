namespace Payroll
{
    public class ChangeDirectTransaction : ChangeMethodTransaction
    {
        protected override PaymentMethod Method => new DirectDepositMethod("Bank -1", "123");

        public ChangeDirectTransaction(int empId, PayrollDatabase database)
            : base(empId, database)
        {
        }
    }
}