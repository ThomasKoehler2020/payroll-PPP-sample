using System;
using System.Collections;

namespace Payroll
{
    public class Paycheck
    {
        private readonly Hashtable fields = new Hashtable();

        public DateTime PayDate { get; }

        public double GrossPay { get; set; }

        public double Deductions { get; set; }

        public double NetPay { get; set; }

        public DateTime PayPeriodEndDate => PayDate;

        public DateTime PayPeriodStartDate { get; }

        public Paycheck(DateTime payPeriodStartDate, DateTime payDate)
        {
            PayDate = payDate;
            PayPeriodStartDate = payPeriodStartDate;
        }

        public void SetField(string fieldName, string value)
        {
            fields[fieldName] = value;
        }

        public string GetField(string fieldName)
        {
            return fields[fieldName] as string;
        }
    }
}