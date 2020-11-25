using System;

namespace Payroll
{
    public class TimeCard
    {
        public double Hours { get; }

        public DateTime Date { get; }

        public TimeCard(DateTime date, double hours)
        {
            this.Date = date;
            this.Hours = hours;
        }
    }
}