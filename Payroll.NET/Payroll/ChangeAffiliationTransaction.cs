namespace Payroll
{
    public abstract class ChangeAffiliationTransaction : ChangeEmployeeTransaction
    {
        protected abstract Affiliation Affiliation { get; }

        public ChangeAffiliationTransaction(int empId, PayrollDatabase database)
            : base(empId, database)
        {
        }

        protected override void Change(Employee e)
        {
            RecordMembership(e);
            var affiliation = Affiliation;
            e.Affiliation = affiliation;
        }

        protected abstract void RecordMembership(Employee e);
    }
}