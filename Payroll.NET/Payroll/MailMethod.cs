namespace Payroll
{
    public class MailMethod : PaymentMethod
    {
        public string Address { get; }

        public MailMethod(string address)
        {
            this.Address = address;
        }

        public void Pay(Paycheck paycheck)
        {
            paycheck.SetField("Disposition", "Mail");
        }

        public override string ToString()
        {
            return string.Format("mail ({0})", Address);
        }
    }
}