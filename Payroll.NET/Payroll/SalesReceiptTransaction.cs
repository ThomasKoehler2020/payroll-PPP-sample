using System;

namespace Payroll
{
    public class SalesReceiptTransaction : Transaction
    {
        private readonly DateTime date;
        private readonly int empId;
        private readonly double saleAmount;

        public SalesReceiptTransaction(DateTime time, double saleAmount, int empId, PayrollDatabase database)
            : base(database)
        {
            date = time;
            this.saleAmount = saleAmount;
            this.empId = empId;
        }

        public override void Execute()
        {
            var e = database.GetEmployee(empId);

            if (e != null)
            {
                var hc =
                    e.Classification as CommissionClassification;

                if (hc != null)
                {
                    hc.AddSalesReceipt(new SalesReceipt(date, saleAmount));
                }
                else
                {
                    throw new ApplicationException(
                        "Tried to add sales receipt to" +
                        "non-commissioned employee");
                }
            }
            else
            {
                throw new ApplicationException(
                    "No such employee.");
            }
        }
    }
}