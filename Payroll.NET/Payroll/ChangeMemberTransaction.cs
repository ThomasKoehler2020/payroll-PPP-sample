namespace Payroll
{
    public class ChangeMemberTransaction : ChangeAffiliationTransaction
    {
        private readonly double dues;
        private readonly int memberId;

        protected override Affiliation Affiliation => new UnionAffiliation(memberId, dues);

        public ChangeMemberTransaction(int empId, int memberId, double dues, PayrollDatabase database)
            : base(empId, database)
        {
            this.memberId = memberId;
            this.dues = dues;
        }

        protected override void RecordMembership(Employee e)
        {
            database.AddUnionMember(memberId, e);
        }
    }
}