namespace Payroll
{
    public class SalariedClassification : PaymentClassification
    {
        public double Salary { get; }

        public SalariedClassification(double salary)
        {
            this.Salary = salary;
        }

        public override double CalculatePay(Paycheck paycheck)
        {
            return Salary;
        }

        public override string ToString()
        {
            return string.Format("${0}", Salary);
        }
    }
}