using System;

namespace Payroll
{
    public class ServiceCharge
    {
        public double Amount { get; }

        public DateTime Time { get; }

        public ServiceCharge(DateTime time, double amount)
        {
            this.Time = time;
            this.Amount = amount;
        }
    }
}