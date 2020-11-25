using System;
using System.Collections;

namespace Payroll
{
    public class UnionAffiliation : Affiliation
    {
        private readonly Hashtable charges = new Hashtable();

        public double Dues { get; }

        public int MemberId { get; }

        public UnionAffiliation(int memberId, double dues)
        {
            MemberId = memberId;
            Dues = dues;
        }

        public UnionAffiliation()
            : this(-1, 0.0)
        {
        }

        public ServiceCharge GetServiceCharge(DateTime time)
        {
            return charges[time] as ServiceCharge;
        }

        public void AddServiceCharge(ServiceCharge sc)
        {
            charges[sc.Time] = sc;
        }

        public double CalculateDeductions(Paycheck paycheck)
        {
            double totalDues = 0;

            var fridays = NumberOfFridaysInPayPeriod(
                paycheck.PayPeriodStartDate, paycheck.PayPeriodEndDate);
            totalDues = Dues * fridays;

            foreach (ServiceCharge charge in charges.Values)
            {
                if (DateUtil.IsInPayPeriod(charge.Time,
                    paycheck.PayPeriodStartDate,
                    paycheck.PayPeriodEndDate))
                {
                    totalDues += charge.Amount;
                }
            }

            return totalDues;
        }

        private int NumberOfFridaysInPayPeriod(
            DateTime payPeriodStart, DateTime payPeriodEnd)
        {
            var fridays = 0;
            for (var day = payPeriodStart;
                day <= payPeriodEnd;
                day = day.AddDays(1))
            {
                if (day.DayOfWeek == DayOfWeek.Friday)
                {
                    fridays++;
                }
            }

            return fridays;
        }
    }
}