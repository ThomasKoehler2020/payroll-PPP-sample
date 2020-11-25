using System.Data;
using NUnit.Framework;
using Payroll;

namespace PayrollDB
{
    [TestFixture]
    public class LoadEmployeeOperationTest
    {
        [SetUp]
        public void SetUp()
        {
            employee = new Employee(123, "Jean", "10 Rue de Roi");
            operation = new LoadEmployeeOperation(123, null);

            operation.Employee = employee;
        }

        private LoadEmployeeOperation operation;
        private Employee employee;

        public static DataRow ShuntRow(string columns, params object[] values)
        {
            var table = new DataTable();
            foreach (var columnName in columns.Split(','))
            {
                table.Columns.Add(columnName);
            }

            return table.Rows.Add(values);
        }

        [Test]
        public void LoadEmployeeData()
        {
            var row = ShuntRow("Name,Address", "Jean", "10 Rue de Roi");

            operation.CreateEmplyee(row);

            Assert.IsNotNull(operation.Employee);
            Assert.AreEqual("Jean", operation.Employee.Name);
            Assert.AreEqual("10 Rue de Roi", operation.Employee.Address);
        }

        [Test]
        public void LoadingEmployeeDataCommand()
        {
            operation = new LoadEmployeeOperation(123, null);
            var command = operation.LoadEmployeeCommand;
            Assert.AreEqual("select * from Employee " +
                            "where EmpId=@EmpId", command.CommandText);
            Assert.AreEqual(123, command.Parameters["@EmpId"].Value);
        }

        [Test]
        public void LoadingSchedules()
        {
            var row = ShuntRow("ScheduleType", "weekly");
            operation.AddSchedule(row);
            Assert.IsTrue(employee.Schedule is WeeklySchedule);

            row = ShuntRow("ScheduleType", "biweekly");
            operation.AddSchedule(row);
            Assert.IsTrue(employee.Schedule is BiWeeklySchedule);

            row = ShuntRow("ScheduleType", "monthly");
            operation.AddSchedule(row);
            Assert.IsTrue(employee.Schedule is MonthlySchedule);
        }
    }
}