using System;

namespace Payroll
{
    public class ServiceChargeTransaction : Transaction
    {
        private readonly double charge;
        private readonly int memberId;
        private readonly DateTime time;

        public ServiceChargeTransaction(int id, DateTime time, double charge, PayrollDatabase database)
            : base(database)
        {
            memberId = id;
            this.time = time;
            this.charge = charge;
        }

        public override void Execute()
        {
            var e = database.GetUnionMember(memberId);

            if (e != null)
            {
                UnionAffiliation ua = null;
                if (e.Affiliation is UnionAffiliation)
                {
                    ua = e.Affiliation as UnionAffiliation;
                }

                if (ua != null)
                {
                    ua.AddServiceCharge(
                        new ServiceCharge(time, charge));
                }
                else
                {
                    throw new ApplicationException(
                        "Tries to add service charge to union"
                        + "member without a union affiliation");
                }
            }
            else
            {
                throw new ApplicationException(
                    "No such union member.");
            }
        }
    }
}