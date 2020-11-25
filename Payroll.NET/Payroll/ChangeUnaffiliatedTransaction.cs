namespace Payroll
{
    public class ChangeUnaffiliatedTransaction : ChangeAffiliationTransaction
    {
        protected override Affiliation Affiliation => new NoAffiliation();

        public ChangeUnaffiliatedTransaction(int empId, PayrollDatabase database)
            : base(empId, database)
        {
        }

        protected override void RecordMembership(Employee e)
        {
            var affiliation = e.Affiliation;
            if (affiliation is UnionAffiliation)
            {
                var unionAffiliation =
                    affiliation as UnionAffiliation;
                var memberId = unionAffiliation.MemberId;
                database.RemoveUnionMember(memberId);
            }
        }
    }
}