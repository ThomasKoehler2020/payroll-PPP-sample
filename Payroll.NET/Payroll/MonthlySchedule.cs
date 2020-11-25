using System;

namespace Payroll
{
    public class MonthlySchedule : PaymentSchedule
    {
        public bool IsPayDate(DateTime payDate)
        {
            return IsLastDayOfMonth(payDate);
        }

        public DateTime GetPayPeriodStartDate(DateTime date)
        {
            var days = 0;
            while (date.AddDays(days - 1).Month == date.Month)
            {
                days--;
            }

            return date.AddDays(days);
        }

        public override string ToString()
        {
            return "monthly";
        }

        private bool IsLastDayOfMonth(DateTime date)
        {
            var m1 = date.Month;
            var m2 = date.AddDays(1).Month;
            return m1 != m2;
        }
    }
}