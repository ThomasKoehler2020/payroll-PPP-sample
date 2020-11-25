namespace Payroll
{
    public abstract class ChangeMethodTransaction : ChangeEmployeeTransaction
    {
        protected abstract PaymentMethod Method { get; }

        public ChangeMethodTransaction(int empId, PayrollDatabase database)
            : base(empId, database)
        {
        }

        protected override void Change(Employee e)
        {
            var method = Method;
            e.Method = method;
        }
    }
}