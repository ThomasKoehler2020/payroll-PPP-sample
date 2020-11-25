using System;
using System.Text;

namespace Payroll
{
    public class Employee
    {
        public string Name { get; set; }

        public string Address { get; }

        public PaymentClassification Classification { get; set; }

        public PaymentSchedule Schedule { get; set; }

        public PaymentMethod Method { get; set; }

        public Affiliation Affiliation { get; set; } = new NoAffiliation();

        public int EmpId { get; }

        public Employee(int empid, string name, string address)
        {
            this.EmpId = empid;
            this.Name = name;
            this.Address = address;
        }

        public bool IsPayDate(DateTime date)
        {
            return Schedule.IsPayDate(date);
        }

        public void Payday(Paycheck paycheck)
        {
            var grossPay = Classification.CalculatePay(paycheck);
            var deductions = Affiliation.CalculateDeductions(paycheck);
            var netPay = grossPay - deductions;
            paycheck.GrossPay = grossPay;
            paycheck.Deductions = deductions;
            paycheck.NetPay = netPay;
            Method.Pay(paycheck);
        }

        public DateTime GetPayPeriodStartDate(DateTime date)
        {
            return Schedule.GetPayPeriodStartDate(date);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Emp#: ").Append(EmpId).Append("   ");
            builder.Append(Name).Append("   ");
            builder.Append(Address).Append("   ");
            builder.Append("Paid ").Append(Classification).Append(" ");
            builder.Append(Schedule);
            builder.Append(" by ").Append(Method);
            return builder.ToString();
        }
    }
}