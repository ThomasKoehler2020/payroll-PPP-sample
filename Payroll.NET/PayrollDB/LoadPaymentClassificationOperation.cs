using System;
using System.Data;
using System.Data.SqlClient;
using Payroll;

namespace PayrollDB
{
    public class LoadPaymentClassificationOperation
    {
        private readonly string classificationType;
        private readonly SqlConnection connection;
        private readonly Employee employee;
        private ClassificationCreator classificationCreator;
        private string tableName;

        public SqlCommand Command
        {
            get
            {
                var sql = string.Format("select * from {0} where EmpId=@EmpId", tableName);
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EmpId", employee.EmpId);
                return command;
            }
        }

        public PaymentClassification Classification { get; private set; }

        public LoadPaymentClassificationOperation(Employee employee, string classificationType, SqlConnection connection)
        {
            this.employee = employee;
            this.classificationType = classificationType;
            this.connection = connection;
        }

        public void Execute()
        {
            Prepare();
            var row = LoadData();
            CreateClassification(row);
        }

        public void Prepare()
        {
            if (classificationType.Equals("hourly"))
            {
                tableName = "HourlyClassification";
                classificationCreator = CreateHourly;
            }
            else if (classificationType.Equals("salary"))
            {
                tableName = "SalariedClassification";
                classificationCreator = CreateSalaried;
            }
            else if (classificationType.Equals("commission"))
            {
                tableName = "CommissionedClassification";
                classificationCreator = CreateCommissioned;
            }
        }

        public void CreateClassification(DataRow row)
        {
            classificationCreator(row);
        }

        private DataRow LoadData()
        {
            if (tableName != null)
            {
                return LoadEmployeeOperation.LoadDataFromCommand(Command);
            }

            return null;
        }

        private void CreateHourly(DataRow row)
        {
            var rate = Convert.ToDouble(row["HourlyRate"]);
            Classification = new HourlyClassification(rate);
        }

        private void CreateSalaried(DataRow row)
        {
            var salary = Convert.ToDouble(row["Salary"]);
            Classification = new SalariedClassification(salary);
        }

        private void CreateCommissioned(DataRow row)
        {
            var baseRate = Convert.ToDouble(row["Salary"]);
            var commissionRate = Convert.ToDouble(row["Commission"]);
            Classification = new CommissionClassification(baseRate, commissionRate);
        }

        private delegate void ClassificationCreator(DataRow row);
    }
}