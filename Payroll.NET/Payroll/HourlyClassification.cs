using System;
using System.Collections;

namespace Payroll
{
    public class HourlyClassification : PaymentClassification
    {
        private readonly Hashtable timeCards = new Hashtable();

        public double HourlyRate { get; }

        public HourlyClassification(double rate)
        {
            HourlyRate = rate;
        }

        public TimeCard GetTimeCard(DateTime date)
        {
            return timeCards[date] as TimeCard;
        }

        public void AddTimeCard(TimeCard card)
        {
            timeCards[card.Date] = card;
        }

        public override double CalculatePay(Paycheck paycheck)
        {
            var totalPay = 0.0;
            foreach (TimeCard timeCard in timeCards.Values)
            {
                if (DateUtil.IsInPayPeriod(timeCard.Date,
                    paycheck.PayPeriodStartDate,
                    paycheck.PayPeriodEndDate))
                {
                    totalPay += CalculatePayForTimeCard(timeCard);
                }
            }

            return totalPay;
        }

        public override string ToString()
        {
            return string.Format("${0}/hr", HourlyRate);
        }

        private double CalculatePayForTimeCard(TimeCard card)
        {
            var overtimeHours = Math.Max(0.0, card.Hours - 8);
            var normalHours = card.Hours - overtimeHours;
            return HourlyRate * normalHours +
                   HourlyRate * 1.5 * overtimeHours;
        }
    }
}