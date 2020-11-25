namespace Payroll
{
    public class ChangeMailTransaction : ChangeMethodTransaction
    {
        protected override PaymentMethod Method => new MailMethod("3.14 Pi St");

        public ChangeMailTransaction(int empId, PayrollDatabase database)
            : base(empId, database)
        {
        }
    }
}