using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Payroll;

namespace PayrollDB
{
    [TestFixture]
    public class SqlPayrollDatabaseTest
    {
        [SetUp]
        public void SetUp()
        {
            database = new SqlPayrollDatabase();
            connection = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Payroll;Trusted_Connection=True");
            connection.Open();

            ClearTables();

            employee = new Employee(123, "George", "123 Baker St.");
            employee.Schedule = new MonthlySchedule();
            employee.Method = new DirectDepositMethod("Bank Of PPP", "27182718");
            employee.Classification = new SalariedClassification(1000.00);
        }

        [TearDown]
        public void TearDown()
        {
            connection.Close();
        }

        private SqlPayrollDatabase database;
        private SqlConnection connection;
        private Employee employee;

        private void ClearTables()
        {
            ClearTable("SalariedClassification");
            ClearTable("CommissionedClassification");
            ClearTable("HourlyClassification");
            ClearTable("PaycheckAddress");
            ClearTable("DirectDepositAccount");
            ClearTable("Employee");
        }

        private void ClearTable(string tableName)
        {
            new SqlCommand("delete from " + tableName, connection).ExecuteNonQuery();
        }

        private DataTable LoadTable(string tableName)
        {
            var command = new SqlCommand("select * from " + tableName, connection);
            var adapter = new SqlDataAdapter(command);
            var dataset = new DataSet();
            adapter.Fill(dataset);
            return dataset.Tables["table"];
        }

        private void CheckSavedScheduleCode(PaymentSchedule schedule, string expectedCode)
        {
            employee.Schedule = schedule;
            database.AddEmployee(employee);

            var table = LoadTable("Employee");
            var row = table.Rows[0];

            Assert.AreEqual(expectedCode, row["ScheduleType"]);
        }

        private void CheckSavedPaymentMethodCode(PaymentMethod method, string expectedCode)
        {
            employee.Method = method;
            database.AddEmployee(employee);

            var table = LoadTable("Employee");
            var row = table.Rows[0];

            Assert.AreEqual(expectedCode, row["PaymentMethodType"]);
        }

        private void CheckSavedClassificationCode(PaymentClassification classification, string expectedCode)
        {
            employee.Classification = classification;
            database.AddEmployee(employee);

            var table = LoadTable("Employee");
            var row = table.Rows[0];

            Assert.AreEqual(expectedCode, row["PaymentClassificationType"]);
        }

        [Test]
        public void AddEmployee()
        {
            database.AddEmployee(employee);

            var table = LoadTable("Employee");

            Assert.AreEqual(1, table.Rows.Count);
            var row = table.Rows[0];
            Assert.AreEqual(123, row["EmpId"]);
            Assert.AreEqual("George", row["Name"]);
            Assert.AreEqual("123 Baker St.", row["Address"]);
        }

        [Test]
        public void Blah()
        {
        }

        [Test]
        public void CommissionClassificationGetsSaved()
        {
            CheckSavedClassificationCode(new CommissionClassification(900.01, 15.5), "commission");

            var table = LoadTable("CommissionedClassification");

            Assert.AreEqual(1, table.Rows.Count);
            var row = table.Rows[0];
            Assert.AreEqual(900.01, Convert.ToDouble(row["Salary"]), 0.01);
            Assert.AreEqual(15.5, Convert.ToDouble(row["Commission"]), 0.01);
            Assert.AreEqual(123, row["EmpId"]);
        }

        [Test]
        public void DirectDepositMethodGetsSaved()
        {
            CheckSavedPaymentMethodCode(new DirectDepositMethod("Bank -1", "0987654321"), "directdeposit");

            var table = LoadTable("DirectDepositAccount");

            Assert.AreEqual(1, table.Rows.Count);
            var row = table.Rows[0];
            Assert.AreEqual("Bank -1", row["Bank"]);
            Assert.AreEqual("0987654321", row["Account"]);
            Assert.AreEqual(123, row["EmpId"]);
        }

        [Test]
        public void HoldMethodGetsSaved()
        {
            CheckSavedPaymentMethodCode(new HoldMethod(), "hold");
        }

        [Test]
        public void HoulyClassificationGetsSaved()
        {
            CheckSavedClassificationCode(new HourlyClassification(7.50), "hourly");

            var table = LoadTable("HourlyClassification");

            Assert.AreEqual(1, table.Rows.Count);
            var row = table.Rows[0];
            Assert.AreEqual(7.50, Convert.ToDouble(row["HourlyRate"]), 0.01);
            Assert.AreEqual(123, row["EmpId"]);
        }

        [Test]
        public void LoadEmployee()
        {
            employee.Schedule = new BiWeeklySchedule();
            employee.Method = new DirectDepositMethod("1st Bank", "0123456");
            employee.Classification = new SalariedClassification(5432.10);
            database.AddEmployee(employee);

            var loadedEmployee = database.GetEmployee(123);
            Assert.AreEqual(123, loadedEmployee.EmpId);
            Assert.AreEqual(employee.Name, loadedEmployee.Name);
            Assert.AreEqual(employee.Address, loadedEmployee.Address);

            var schedule = loadedEmployee.Schedule;
            Assert.IsTrue(schedule is BiWeeklySchedule);

            var method = loadedEmployee.Method;
            Assert.IsTrue(method is DirectDepositMethod);
            var ddMethod = method as DirectDepositMethod;
            Assert.AreEqual("1st Bank", ddMethod.Bank);
            Assert.AreEqual("0123456", ddMethod.AccountNumber);

            var classification = loadedEmployee.Classification;
            Assert.IsTrue(classification is SalariedClassification);
            var salariedClassification = classification as SalariedClassification;
            Assert.AreEqual(5432.10, salariedClassification.Salary);
        }

        [Test]
        public void MailMethodGetsSaved()
        {
            CheckSavedPaymentMethodCode(new MailMethod("111 Maple Ct."), "mail");

            var table = LoadTable("PaycheckAddress");

            Assert.AreEqual(1, table.Rows.Count);
            var row = table.Rows[0];
            Assert.AreEqual("111 Maple Ct.", row["Address"]);
            Assert.AreEqual(123, row["EmpId"]);
        }

        [Test]
        public void SalariedClassificationGetsSaved()
        {
            CheckSavedClassificationCode(new SalariedClassification(1234.56), "salary");

            var table = LoadTable("SalariedClassification");

            Assert.AreEqual(1, table.Rows.Count);
            var row = table.Rows[0];
            Assert.AreEqual(1234.56, Convert.ToDouble(row["Salary"]), 0.01);
            Assert.AreEqual(123, row["EmpId"]);
        }

        [Test]
        public void SaveIsTransactional()
        {
            // Null values won't go in the database.
            var method = new DirectDepositMethod(null, null);
            employee.Method = method;
            try
            {
                database.AddEmployee(employee);
                Assert.Fail("An exception needs to occur for this test to work.");
            }
            catch (SqlException)
            {
            }

            var table = LoadTable("Employee");
            Assert.AreEqual(0, table.Rows.Count);
        }

        [Test]
        public void SaveMailMethodThenHoldMethod()
        {
            employee.Method = new MailMethod("123 Baker St.");
            database.AddEmployee(employee);

            var employee2 = new Employee(321, "Ed", "456 Elm St.");
            employee2.Method = new HoldMethod();
            database.AddEmployee(employee2);

            var table = LoadTable("PaycheckAddress");
            Assert.AreEqual(1, table.Rows.Count);
        }

        [Test]
        public void ScheduleGetsSaved()
        {
            CheckSavedScheduleCode(new MonthlySchedule(), "monthly");
            ClearTables();
            CheckSavedScheduleCode(new WeeklySchedule(), "weekly");
            ClearTables();
            CheckSavedScheduleCode(new BiWeeklySchedule(), "biweekly");
        }
    }
}