using System;
using System.Collections;

namespace Payroll
{
    public class CommissionClassification : PaymentClassification
    {
        private readonly Hashtable salesReceipts = new Hashtable();

        public double BaseRate { get; }

        public double CommissionRate { get; }

        public CommissionClassification(double baseRate, double commissionRate)
        {
            this.BaseRate = baseRate;
            this.CommissionRate = commissionRate;
        }

        public void AddSalesReceipt(SalesReceipt receipt)
        {
            salesReceipts[receipt.Date] = receipt;
        }

        public SalesReceipt GetSalesReceipt(DateTime time)
        {
            return salesReceipts[time] as SalesReceipt;
        }

        public override double CalculatePay(Paycheck paycheck)
        {
            double salesTotal = 0;
            foreach (SalesReceipt receipt in salesReceipts.Values)
            {
                if (DateUtil.IsInPayPeriod(receipt.Date,
                    paycheck.PayPeriodStartDate,
                    paycheck.PayPeriodEndDate))
                {
                    salesTotal += receipt.SaleAmount;
                }
            }

            return BaseRate + salesTotal * CommissionRate * 0.01;
        }

        public override string ToString()
        {
            return string.Format("${0} + {1}% sales commission", BaseRate, CommissionRate);
        }
    }
}