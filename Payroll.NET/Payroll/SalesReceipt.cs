using System;

namespace Payroll
{
    public class SalesReceipt
    {
        public DateTime Date { get; }

        public double SaleAmount { get; }

        public SalesReceipt(DateTime date, double amount)
        {
            this.Date = date;
            SaleAmount = amount;
        }
    }
}