namespace Payroll
{
    public class DirectDepositMethod : PaymentMethod
    {
        public string Bank { get; }

        public string AccountNumber { get; }

        public DirectDepositMethod(string bank, string accountNumber)
        {
            this.Bank = bank;
            this.AccountNumber = accountNumber;
        }

        public void Pay(Paycheck paycheck)
        {
            paycheck.SetField("Disposition", "Direct");
        }

        public override string ToString()
        {
            return string.Format("direct deposit into {0}:{1}", Bank, AccountNumber);
        }
    }
}