namespace Payroll
{
    public abstract class AddEmployeeTransaction : Transaction
    {
        private readonly string address;
        private readonly int empid;
        private readonly string name;

        public AddEmployeeTransaction(int empid,
            string name, string address, PayrollDatabase database)
            : base(database)
        {
            this.empid = empid;
            this.name = name;
            this.address = address;
        }

        protected abstract
            PaymentClassification MakeClassification();

        protected abstract
            PaymentSchedule MakeSchedule();

        public override void Execute()
        {
            var pc = MakeClassification();
            var ps = MakeSchedule();
            PaymentMethod pm = new HoldMethod();

            var e = new Employee(empid, name, address);
            e.Classification = pc;
            e.Schedule = ps;
            e.Method = pm;
            database.AddEmployee(e);
        }

        public override string ToString()
        {
            return string.Format("{0}  id:{1}   name:{2}   address:{3}", GetType().Name, empid, name, address);
        }
    }
}