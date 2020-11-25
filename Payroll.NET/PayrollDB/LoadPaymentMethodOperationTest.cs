using NUnit.Framework;
using Payroll;

namespace PayrollDB
{
    [TestFixture]
    public class LoadPaymentMethodOperationTest
    {
        [SetUp]
        public void SetUp()
        {
            employee = new Employee(567, "Bill", "23 Pine Ct");
        }

        private Employee employee;
        private LoadPaymentMethodOperation operation;

        [Test]
        public void CreateDirectDepositMethodFromRow()
        {
            operation = new LoadPaymentMethodOperation(employee, "directdeposit", null);
            operation.Prepare();
            var row = LoadEmployeeOperationTest.ShuntRow("Bank,Account", "1st Bank", "0123456");
            operation.CreatePaymentMethod(row);

            var method = operation.Method;
            Assert.IsTrue(method is DirectDepositMethod);
            var ddMethod = method as DirectDepositMethod;
            Assert.AreEqual("1st Bank", ddMethod.Bank);
            Assert.AreEqual("0123456", ddMethod.AccountNumber);
        }

        [Test]
        public void CreateMailMethodFromRow()
        {
            operation = new LoadPaymentMethodOperation(employee, "mail", null);
            operation.Prepare();
            var row = LoadEmployeeOperationTest.ShuntRow("Address", "23 Pine Ct");
            operation.CreatePaymentMethod(row);

            var method = operation.Method;
            Assert.IsTrue(method is MailMethod);
            var mailMethod = method as MailMethod;
            Assert.AreEqual("23 Pine Ct", mailMethod.Address);
        }

        [Test]
        public void LoadDirectDepositMethodCommand()
        {
            operation = new LoadPaymentMethodOperation(employee, "directdeposit", null);
            operation.Prepare();
            var command = operation.Command;
            Assert.AreEqual("select * from DirectDepositAccount where EmpId=@EmpId", command.CommandText);
            Assert.AreEqual(employee.EmpId, command.Parameters["@EmpId"].Value);
        }

        [Test]
        public void LoadHoldMethod()
        {
            operation = new LoadPaymentMethodOperation(
                employee, "hold", null);
            operation.Execute();
            var method = operation.Method;
            Assert.IsTrue(method is HoldMethod);
        }

        [Test]
        public void LoadMailMethodCommand()
        {
            operation = new LoadPaymentMethodOperation(employee, "mail", null);
            operation.Prepare();
            var command = operation.Command;
            Assert.AreEqual("select * from PaycheckAddress where EmpId=@EmpId", command.CommandText);
            Assert.AreEqual(employee.EmpId, command.Parameters["@EmpId"].Value);
        }
    }
}