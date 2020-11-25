using System;

namespace Payroll
{
    public class TimeCardTransaction : Transaction
    {
        private readonly DateTime date;
        private readonly int empId;
        private readonly double hours;

        public TimeCardTransaction(DateTime date, double hours, int empId, PayrollDatabase database)
            : base(database)
        {
            this.date = date;
            this.hours = hours;
            this.empId = empId;
        }

        public override void Execute()
        {
            var e = database.GetEmployee(empId);

            if (e != null)
            {
                var hc =
                    e.Classification as HourlyClassification;

                if (hc != null)
                {
                    hc.AddTimeCard(new TimeCard(date, hours));
                }
                else
                {
                    throw new ApplicationException(
                        "Tried to add timecard to" +
                        "non-hourly employee");
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