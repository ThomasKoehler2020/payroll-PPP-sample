using System.Data;
using System.Data.SqlClient;
using Payroll;

namespace PayrollDB
{
    public class LoadEmployeeOperation
    {
        private readonly SqlConnection connection;
        private readonly int empId;

        public SqlCommand LoadEmployeeCommand
        {
            get
            {
                var sql = "select * from Employee " +
                          "where EmpId=@EmpId";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EmpId", empId);
                return command;
            }
        }

        public Employee Employee { get; set; }

        public LoadEmployeeOperation(
            int empId, SqlConnection connection)
        {
            this.empId = empId;
            this.connection = connection;
        }

        public void Execute()
        {
            var sql = "select *  from Employee where EmpId = @EmpId";
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EmpId", empId);

            var row = LoadDataFromCommand(command);

            CreateEmplyee(row);
            AddSchedule(row);
            AddPaymentMethod(row);
            AddClassification(row);
        }

        public void AddSchedule(DataRow row)
        {
            var scheduleType = row["ScheduleType"].ToString();
            if (scheduleType.Equals("weekly"))
            {
                Employee.Schedule = new WeeklySchedule();
            }
            else if (scheduleType.Equals("biweekly"))
            {
                Employee.Schedule = new BiWeeklySchedule();
            }
            else if (scheduleType.Equals("monthly"))
            {
                Employee.Schedule = new MonthlySchedule();
            }
        }

        public void CreateEmplyee(DataRow row)
        {
            var name = row["Name"].ToString();
            var address = row["Address"].ToString();
            Employee = new Employee(empId, name, address);
        }

        public static DataRow LoadDataFromCommand(SqlCommand command)
        {
            var adapter = new SqlDataAdapter(command);
            var dataset = new DataSet();
            adapter.Fill(dataset);
            var table = dataset.Tables["table"];
            return table.Rows[0];
        }

        private void AddPaymentMethod(DataRow row)
        {
            var methodCode = row["PaymentMethodType"].ToString();
            var operation = new LoadPaymentMethodOperation(Employee, methodCode, connection);
            operation.Execute();
            Employee.Method = operation.Method;
        }

        private void AddClassification(DataRow row)
        {
            var classificationCode = row["PaymentClassificationType"].ToString();
            var operation = new LoadPaymentClassificationOperation(Employee, classificationCode, connection);
            operation.Execute();
            Employee.Classification = operation.Classification;
        }
    }
}