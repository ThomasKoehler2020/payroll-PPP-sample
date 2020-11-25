using System;
using NUnit.Framework;

namespace Payroll
{
    [TestFixture]
    public class PayrollTest
    {
        [SetUp]
        public void SetUp()
        {
            database = new InMemoryPayrollDatabase();
        }

        private PayrollDatabase database;

        private void ValidatePaycheck(PaydayTransaction pt,
            int empid, DateTime payDate, double pay)
        {
            var pc = pt.GetPaycheck(empid);
            Assert.IsNotNull(pc);
            Assert.AreEqual(payDate, pc.PayDate);
            Assert.AreEqual(pay, pc.GrossPay, .001);
            Assert.AreEqual("Hold", pc.GetField("Disposition"));
            Assert.AreEqual(0.0, pc.Deductions, .001);
            Assert.AreEqual(pay, pc.NetPay, .001);
        }

        [Test]
        public void AddServiceCharge()
        {
            var empId = 2;
            var t = new AddHourlyEmployee(
                empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            var af = new UnionAffiliation();
            e.Affiliation = af;
            var memberId = 86; // Maxwell Smart
            database.AddUnionMember(memberId, e);
            var sct =
                new ServiceChargeTransaction(
                    memberId, new DateTime(2005, 8, 8), 12.95, database);
            sct.Execute();
            var sc =
                af.GetServiceCharge(new DateTime(2005, 8, 8));
            Assert.IsNotNull(sc);
            Assert.AreEqual(12.95, sc.Amount, .001);
        }

        [Test]
        public void ChangeDirectMethod()
        {
            var empId = 6;
            var t =
                new AddSalariedEmployee(
                    empId, "Mike", "Home", 3500.00, database);
            t.Execute();
            var cddt =
                new ChangeDirectTransaction(empId, database);
            cddt.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            var method = e.Method;
            Assert.IsNotNull(method);
            Assert.IsTrue(method is DirectDepositMethod);
        }

        [Test]
        public void ChangeHoldMethod()
        {
            var empId = 7;
            var t =
                new AddSalariedEmployee(
                    empId, "Mike", "Home", 3500.00, database);
            t.Execute();
            new ChangeDirectTransaction(empId, database).Execute();
            var cht =
                new ChangeHoldTransaction(empId, database);
            cht.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            var method = e.Method;
            Assert.IsNotNull(method);
            Assert.IsTrue(method is HoldMethod);
        }

        [Test]
        public void ChangeMailMethod()
        {
            var empId = 8;
            var t =
                new AddSalariedEmployee(
                    empId, "Mike", "Home", 3500.00, database);
            t.Execute();
            var cmt =
                new ChangeMailTransaction(empId, database);
            cmt.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            var method = e.Method;
            Assert.IsNotNull(method);
            Assert.IsTrue(method is MailMethod);
        }

        [Test]
        public void ChangeUnaffiliatedMember()
        {
            var empId = 10;
            var t =
                new AddHourlyEmployee(empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var memberId = 7743;
            new ChangeMemberTransaction(empId, memberId, 99.42, database).Execute();
            var cut =
                new ChangeUnaffiliatedTransaction(empId, database);
            cut.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            var affiliation = e.Affiliation;
            Assert.IsNotNull(affiliation);
            Assert.IsTrue(affiliation is NoAffiliation);
            var member = database.GetUnionMember(memberId);
            Assert.IsNull(member);
        }

        [Test]
        public void ChangeUnionMember()
        {
            var empId = 9;
            var t =
                new AddHourlyEmployee(empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var memberId = 7743;
            var cmt =
                new ChangeMemberTransaction(empId, memberId, 99.42, database);
            cmt.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            var affiliation = e.Affiliation;
            Assert.IsNotNull(affiliation);
            Assert.IsTrue(affiliation is UnionAffiliation);
            var uf = affiliation as UnionAffiliation;
            Assert.AreEqual(99.42, uf.Dues, .001);
            var member = database.GetUnionMember(memberId);
            Assert.IsNotNull(member);
            Assert.AreEqual(e, member);
        }

        [Test]
        public void DeleteEmplyee()
        {
            var empId = 4;
            var t =
                new AddCommissionedEmployee(
                    empId, "Bill", "Home", 2500, 3.2, database);
            t.Execute();

            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);

            var dt =
                new DeleteEmployeeTransaction(empId, database);
            dt.Execute();

            e = database.GetEmployee(empId);
            Assert.IsNull(e);
        }

        [Test]
        public void HourlyUnionMemberServiceCharge()
        {
            var empId = 1;
            var t = new AddHourlyEmployee(
                empId, "Bill", "Home", 15.24, database);
            t.Execute();
            var memberId = 7734;
            var cmt =
                new ChangeMemberTransaction(empId, memberId, 9.42, database);
            cmt.Execute();
            var payDate = new DateTime(2001, 11, 9);
            var sct =
                new ServiceChargeTransaction(memberId, payDate, 19.42, database);
            sct.Execute();
            var tct =
                new TimeCardTransaction(payDate, 8.0, empId, database);
            tct.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            var pc = pt.GetPaycheck(empId);
            Assert.IsNotNull(pc);
            Assert.AreEqual(payDate, pc.PayPeriodEndDate);
            Assert.AreEqual(8 * 15.24, pc.GrossPay, .001);
            Assert.AreEqual("Hold", pc.GetField("Disposition"));
            Assert.AreEqual(9.42 + 19.42, pc.Deductions, .001);
            Assert.AreEqual(8 * 15.24 - (9.42 + 19.42), pc.NetPay, .001);
        }

        [Test]
        public void PayingSingleCommissionedEmployeeNoReceipts()
        {
            var empId = 2;
            var t = new AddCommissionedEmployee(
                empId, "Bill", "Home", 1500, 10, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 16); // Payday
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            ValidatePaycheck(pt, empId, payDate, 1500.0);
        }

        [Test]
        public void PayingSingleHourlyEmployeeNoTimeCards()
        {
            var empId = 2;
            var t = new AddHourlyEmployee(
                empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 9); // Friday
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            ValidatePaycheck(pt, empId, payDate, 0.0);
        }

        [Test]
        public void PaySingleCommissionedEmployeeOneReceipt()
        {
            var empId = 2;
            var t = new AddCommissionedEmployee(
                empId, "Bill", "Home", 1500, 10, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 16); // Payday

            var sr =
                new SalesReceiptTransaction(payDate, 5000.00, empId, database);
            sr.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            ValidatePaycheck(pt, empId, payDate, 2000.00);
        }

        [Test]
        public void PaySingleCommissionedEmployeeOnWrongDate()
        {
            var empId = 2;
            var t = new AddCommissionedEmployee(
                empId, "Bill", "Home", 1500, 10, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 9); // wrong friday

            var sr =
                new SalesReceiptTransaction(payDate, 5000.00, empId, database);
            sr.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();

            var pc = pt.GetPaycheck(empId);
            Assert.IsNull(pc);
        }

        [Test]
        public void PaySingleCommissionedEmployeeTwoReceipts()
        {
            var empId = 2;
            var t = new AddCommissionedEmployee(
                empId, "Bill", "Home", 1500, 10, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 16); // Payday

            var sr =
                new SalesReceiptTransaction(payDate, 5000.00, empId, database);
            sr.Execute();
            var sr2 = new SalesReceiptTransaction(
                payDate.AddDays(-1), 3500.00, empId, database);
            sr2.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            ValidatePaycheck(pt, empId, payDate, 2350.00);
        }

        [Test]
        public void PaySingleHourlyEmployeeOneTimeCard()
        {
            var empId = 2;
            var t = new AddHourlyEmployee(
                empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 9); // Friday

            var tc =
                new TimeCardTransaction(payDate, 2.0, empId, database);
            tc.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            ValidatePaycheck(pt, empId, payDate, 30.5);
        }

        [Test]
        public void PaySingleHourlyEmployeeOnWrongDate()
        {
            var empId = 2;
            var t = new AddHourlyEmployee(
                empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 8); // Thursday

            var tc =
                new TimeCardTransaction(payDate, 9.0, empId, database);
            tc.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();

            var pc = pt.GetPaycheck(empId);
            Assert.IsNull(pc);
        }

        [Test]
        public void PaySingleHourlyEmployeeOvertimeOneTimeCard()
        {
            var empId = 2;
            var t = new AddHourlyEmployee(
                empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 9); // Friday

            var tc =
                new TimeCardTransaction(payDate, 9.0, empId, database);
            tc.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            ValidatePaycheck(pt, empId, payDate, (8 + 1.5) * 15.25);
        }

        [Test]
        public void PaySingleHourlyEmployeeTwoTimeCards()
        {
            var empId = 2;
            var t = new AddHourlyEmployee(
                empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 9); // Friday

            var tc =
                new TimeCardTransaction(payDate, 2.0, empId, database);
            tc.Execute();
            var tc2 =
                new TimeCardTransaction(payDate.AddDays(-1), 5.0, empId, database);
            tc2.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            ValidatePaycheck(pt, empId, payDate, 7 * 15.25);
        }

        [Test]
        public void PaySingleSalariedEmployee()
        {
            var empId = 1;
            var t = new AddSalariedEmployee(
                empId, "Bob", "Home", 1000.00, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 30);
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            var pc = pt.GetPaycheck(empId);
            Assert.IsNotNull(pc);
            Assert.AreEqual(payDate, pc.PayDate);
            Assert.AreEqual(1000.00, pc.GrossPay, .001);
            Assert.AreEqual("Hold", pc.GetField("Disposition"));
            Assert.AreEqual(0.0, pc.Deductions, .001);
            Assert.AreEqual(1000.00, pc.NetPay, .001);
        }

        [Test]
        public void PaySingleSalariedEmployeeOnWrongDate()
        {
            var empId = 1;
            var t = new AddSalariedEmployee(
                empId, "Bob", "Home", 1000.00, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 29);
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            var pc = pt.GetPaycheck(empId);
            Assert.IsNull(pc);
        }

        [Test]
        public void SalariedUnionMemberDues()
        {
            var empId = 1;
            var t = new AddSalariedEmployee(
                empId, "Bob", "Home", 1000.00, database);
            t.Execute();
            var memberId = 7734;
            var cmt =
                new ChangeMemberTransaction(empId, memberId, 9.42, database);
            cmt.Execute();
            var payDate = new DateTime(2001, 11, 30);
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            var pc = pt.GetPaycheck(empId);
            Assert.IsNotNull(pc);
            Assert.AreEqual(payDate, pc.PayDate);
            Assert.AreEqual(1000.0, pc.GrossPay, .001);
            Assert.AreEqual("Hold", pc.GetField("Disposition"));
            Assert.AreEqual(47.1, pc.Deductions, .001);
            Assert.AreEqual(1000.0 - 47.1, pc.NetPay, .001);
        }

        [Test]
        public void ServiceChargesSpanningMultiplePayPeriods()
        {
            var empId = 1;
            var t = new AddHourlyEmployee(
                empId, "Bill", "Home", 15.24, database);
            t.Execute();
            var memberId = 7734;
            var cmt =
                new ChangeMemberTransaction(empId, memberId, 9.42, database);
            cmt.Execute();
            var payDate = new DateTime(2001, 11, 9);
            var earlyDate =
                new DateTime(2001, 11, 2); // previous Friday
            var lateDate =
                new DateTime(2001, 11, 16); // next Friday
            var sct =
                new ServiceChargeTransaction(memberId, payDate, 19.42, database);
            sct.Execute();
            var sctEarly =
                new ServiceChargeTransaction(memberId, earlyDate, 100.00, database);
            sctEarly.Execute();
            var sctLate =
                new ServiceChargeTransaction(memberId, lateDate, 200.00, database);
            sctLate.Execute();
            var tct =
                new TimeCardTransaction(payDate, 8.0, empId, database);
            tct.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            var pc = pt.GetPaycheck(empId);
            Assert.IsNotNull(pc);
            Assert.AreEqual(payDate, pc.PayPeriodEndDate);
            Assert.AreEqual(8 * 15.24, pc.GrossPay, .001);
            Assert.AreEqual("Hold", pc.GetField("Disposition"));
            Assert.AreEqual(9.42 + 19.42, pc.Deductions, .001);
            Assert.AreEqual(8 * 15.24 - (9.42 + 19.42), pc.NetPay, .001);
        }

        [Test]
        public void TestAddCommissionedEmployee()
        {
            var empId = 3;
            var t =
                new AddCommissionedEmployee(empId, "Justin", "Home", 2500, 9.5, database);
            t.Execute();

            var e = database.GetEmployee(empId);
            Assert.AreEqual("Justin", e.Name);

            var pc = e.Classification;
            Assert.IsTrue(pc is CommissionClassification);
            var cc = pc as CommissionClassification;

            Assert.AreEqual(2500, cc.BaseRate, .001);
            Assert.AreEqual(9.5, cc.CommissionRate, .001);
            var ps = e.Schedule;
            Assert.IsTrue(ps is BiWeeklySchedule);

            var pm = e.Method;
            Assert.IsTrue(pm is HoldMethod);
        }

        [Test]
        public void TestAddHourlyEmployee()
        {
            var empId = 2;
            var t =
                new AddHourlyEmployee(empId, "Micah", "Home", 200.00, database);
            t.Execute();

            var e = database.GetEmployee(empId);
            Assert.AreEqual("Micah", e.Name);

            var pc = e.Classification;
            Assert.IsTrue(pc is HourlyClassification);
            var hc = pc as HourlyClassification;

            Assert.AreEqual(200.00, hc.HourlyRate, .001);
            var ps = e.Schedule;
            Assert.IsTrue(ps is WeeklySchedule);

            var pm = e.Method;
            Assert.IsTrue(pm is HoldMethod);
        }

        [Test]
        public void TestAddSalariedEmployee()
        {
            var empId = 1;
            var t =
                new AddSalariedEmployee(empId, "Bob", "Home", 1000.00, database);
            t.Execute();

            var e = database.GetEmployee(empId);
            Assert.AreEqual("Bob", e.Name);

            var pc = e.Classification;
            Assert.IsTrue(pc is SalariedClassification);
            var sc = pc as SalariedClassification;

            Assert.AreEqual(1000.00, sc.Salary, .001);
            var ps = e.Schedule;
            Assert.IsTrue(ps is MonthlySchedule);

            var pm = e.Method;
            Assert.IsTrue(pm is HoldMethod);
        }

        [Test]
        public void TestChangeCommisionTransaction()
        {
            var empId = 5;
            var t =
                new AddSalariedEmployee(
                    empId, "Bob", "Home", 2500.00, database);
            t.Execute();
            var cht =
                new ChangeCommissionedTransaction(empId, 1250.00, 5.6, database);
            cht.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            var pc = e.Classification;
            Assert.IsNotNull(pc);
            Assert.IsTrue(pc is CommissionClassification);
            var cc = pc as CommissionClassification;
            Assert.AreEqual(1250.00, cc.BaseRate, .001);
            Assert.AreEqual(5.6, cc.CommissionRate, .001);
            var ps = e.Schedule;
            Assert.IsTrue(ps is BiWeeklySchedule);
        }

        [Test]
        public void TestChangeHourlyTransaction()
        {
            var empId = 3;
            var t =
                new AddCommissionedEmployee(
                    empId, "Lance", "Home", 2500, 3.2, database);
            t.Execute();
            var cht =
                new ChangeHourlyTransaction(empId, 27.52, database);
            cht.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            var pc = e.Classification;
            Assert.IsNotNull(pc);
            Assert.IsTrue(pc is HourlyClassification);
            var hc = pc as HourlyClassification;
            Assert.AreEqual(27.52, hc.HourlyRate, .001);
            var ps = e.Schedule;
            Assert.IsTrue(ps is WeeklySchedule);
        }

        [Test]
        public void TestChangeNameTransaction()
        {
            var empId = 2;
            var t = new AddHourlyEmployee(empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var cnt = new ChangeNameTransaction(empId, "Bob", database);
            cnt.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            Assert.AreEqual("Bob", e.Name);
        }

        [Test]
        public void TestChangeSalaryTransaction()
        {
            var empId = 4;
            var t =
                new AddCommissionedEmployee(
                    empId, "Lance", "Home", 2500, 3.2, database);
            t.Execute();
            var cst =
                new ChangeSalariedTransaction(empId, 3000.00, database);
            cst.Execute();
            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);
            var pc = e.Classification;
            Assert.IsNotNull(pc);
            Assert.IsTrue(pc is SalariedClassification);
            var sc = pc as SalariedClassification;
            Assert.AreEqual(3000.00, sc.Salary, .001);
            var ps = e.Schedule;
            Assert.IsTrue(ps is MonthlySchedule);
        }

        [Test]
        public void
            TestPaySingleCommissionedEmployeeWithReceiptsSpanningTwoPayPeriods()
        {
            var empId = 2;
            var t = new AddCommissionedEmployee(
                empId, "Bill", "Home", 1500, 10, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 16); // Payday

            var sr =
                new SalesReceiptTransaction(payDate, 5000.00, empId, database);
            sr.Execute();
            var sr2 = new SalesReceiptTransaction(
                payDate.AddDays(-15), 3500.00, empId, database);
            sr2.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            ValidatePaycheck(pt, empId, payDate, 2000.00);
        }

        [Test]
        public void
            TestPaySingleHourlyEmployeeWithTimeCardsSpanningTwoPayPeriods()
        {
            var empId = 2;
            var t = new AddHourlyEmployee(
                empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var payDate = new DateTime(2001, 11, 9); // Friday
            var dateInPreviousPayPeriod =
                new DateTime(2001, 10, 30);

            var tc =
                new TimeCardTransaction(payDate, 2.0, empId, database);
            tc.Execute();
            var tc2 = new TimeCardTransaction(
                dateInPreviousPayPeriod, 5.0, empId, database);
            tc2.Execute();
            var pt = new PaydayTransaction(payDate, database);
            pt.Execute();
            ValidatePaycheck(pt, empId, payDate, 2 * 15.25);
        }

        [Test]
        public void TestSalesReceiptTransaction()
        {
            var empId = 5;
            var t =
                new AddCommissionedEmployee(
                    empId, "Bill", "Home", 2000, 15.25, database);
            t.Execute();
            var tct =
                new SalesReceiptTransaction(
                    new DateTime(2005, 7, 31), 250.00, empId, database);
            tct.Execute();

            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);

            var pc = e.Classification;
            Assert.IsTrue(pc is CommissionClassification);
            var cc = pc as CommissionClassification;

            var sr = cc.GetSalesReceipt(new DateTime(2005, 7, 31));
            Assert.IsNotNull(sr);
            Assert.AreEqual(250.00, sr.SaleAmount, .001);
        }

        [Test]
        public void TestTimeCardTransaction()
        {
            var empId = 5;
            var t =
                new AddHourlyEmployee(empId, "Bill", "Home", 15.25, database);
            t.Execute();
            var tct =
                new TimeCardTransaction(
                    new DateTime(2005, 7, 31), 8.0, empId, database);
            tct.Execute();

            var e = database.GetEmployee(empId);
            Assert.IsNotNull(e);

            var pc = e.Classification;
            Assert.IsTrue(pc is HourlyClassification);
            var hc = pc as HourlyClassification;

            var tc = hc.GetTimeCard(new DateTime(2005, 7, 31));
            Assert.IsNotNull(tc);
            Assert.AreEqual(8.0, tc.Hours);
        }
    }
}