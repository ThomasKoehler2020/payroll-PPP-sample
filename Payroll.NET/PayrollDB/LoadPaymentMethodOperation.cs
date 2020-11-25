using System.Data;
using System.Data.SqlClient;
using Payroll;

namespace PayrollDB
{
    public class LoadPaymentMethodOperation
    {
        private readonly SqlConnection connection;
        private readonly Employee employee;
        private readonly string methodCode;
        private PaymentMethodCreator paymentMethodCreator;
        private string tableName;

        public PaymentMethod Method { get; private set; }

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

        public LoadPaymentMethodOperation(Employee employee, string methodCode, SqlConnection connection)
        {
            this.employee = employee;
            this.methodCode = methodCode;
            this.connection = connection;
        }

        public void Execute()
        {
            Prepare();
            var row = LoadData();
            CreatePaymentMethod(row);
        }

        public void CreatePaymentMethod(DataRow row)
        {
            paymentMethodCreator(row);
        }

        public void Prepare()
        {
            if (methodCode.Equals("hold"))
            {
                paymentMethodCreator = CreateHoldMethod;
            }
            else if (methodCode.Equals("directdeposit"))
            {
                tableName = "DirectDepositAccount";
                paymentMethodCreator = CreateDirectDepositMethod;
            }
            else if (methodCode.Equals("mail"))
            {
                tableName = "PaycheckAddress";
                paymentMethodCreator = CreateMailMethod;
            }
        }

        public void CreateDirectDepositMethod(DataRow row)
        {
            var bank = row["Bank"].ToString();
            var account = row["Account"].ToString();
            Method = new DirectDepositMethod(bank, account);
        }

        private DataRow LoadData()
        {
            if (tableName != null)
            {
                return LoadEmployeeOperation.LoadDataFromCommand(Command);
            }

            return null;
        }

        private void CreateHoldMethod(DataRow row)
        {
            Method = new HoldMethod();
        }

        private void CreateMailMethod(DataRow row)
        {
            var address = row["Address"].ToString();
            Method = new MailMethod(address);
        }

        private delegate void PaymentMethodCreator(DataRow row);
    }
}