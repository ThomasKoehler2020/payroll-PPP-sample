using System;
using System.Data.SqlClient;
using Payroll;

namespace PayrollDB
{
    public class SaveEmployeeOperation
    {
        private readonly SqlConnection connection;
        private readonly Employee employee;
        private string classificationCode;
        private SqlCommand insertClassificationCommand;
        private SqlCommand insertEmployeeCommand;
        private SqlCommand insertPaymentMethodCommand;

        private string methodCode;

        public SaveEmployeeOperation(Employee employee, SqlConnection connection)
        {
            this.employee = employee;
            this.connection = connection;
        }

        public void Execute()
        {
            PrepareToSavePaymentMethod(employee);
            PrepareToSaveClassification(employee);
            PrepareToSaveEmployee(employee);

            var transaction = connection.BeginTransaction("Save Employee");
            try
            {
                ExecuteCommand(insertEmployeeCommand, transaction);
                ExecuteCommand(insertPaymentMethodCommand, transaction);
                ExecuteCommand(insertClassificationCommand, transaction);
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

        private void ExecuteCommand(SqlCommand command, SqlTransaction transaction)
        {
            if (command != null)
            {
                command.Connection = connection;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
        }

        private void PrepareToSaveEmployee(Employee employee)
        {
            var sql = "insert into Employee values (" +
                      "@EmpId, @Name, @Address, @ScheduleType, " +
                      "@PaymentMethodType, @PaymentClassificationType)";
            insertEmployeeCommand = new SqlCommand(sql);

            insertEmployeeCommand.Parameters.AddWithValue("@EmpId", employee.EmpId);
            insertEmployeeCommand.Parameters.AddWithValue("@Name", employee.Name);
            insertEmployeeCommand.Parameters.AddWithValue("@Address", employee.Address);
            insertEmployeeCommand.Parameters.AddWithValue("@ScheduleType", ScheduleCode(employee.Schedule));
            insertEmployeeCommand.Parameters.AddWithValue("@PaymentMethodType", methodCode);
            insertEmployeeCommand.Parameters.AddWithValue("@PaymentClassificationType", classificationCode);
        }

        private void PrepareToSavePaymentMethod(Employee employee)
        {
            var method = employee.Method;
            if (method is HoldMethod)
            {
                methodCode = "hold";
            }
            else if (method is DirectDepositMethod)
            {
                methodCode = "directdeposit";
                var ddMethod = method as DirectDepositMethod;
                insertPaymentMethodCommand = CreateInsertDirectDepositCommand(ddMethod, employee);
            }
            else if (method is MailMethod)
            {
                methodCode = "mail";
                var mailMethod = method as MailMethod;
                insertPaymentMethodCommand = CreateInsertMailMethodCommand(mailMethod, employee);
            }
            else
            {
                methodCode = "unknown";
            }
        }

        private SqlCommand CreateInsertDirectDepositCommand(DirectDepositMethod ddMethod, Employee employee)
        {
            var sql = "insert into DirectDepositAccount values (@Bank, @Account, @EmpId)";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@Bank", ddMethod.Bank);
            command.Parameters.AddWithValue("@Account", ddMethod.AccountNumber);
            command.Parameters.AddWithValue("@EmpId", employee.EmpId);
            return command;
        }

        private SqlCommand CreateInsertMailMethodCommand(MailMethod mailMethod, Employee employee)
        {
            var sql = "insert into PaycheckAddress values (@Address, @EmpId)";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@Address", mailMethod.Address);
            command.Parameters.AddWithValue("@EmpId", employee.EmpId);
            return command;
        }

        private void PrepareToSaveClassification(Employee employee)
        {
            var classification = employee.Classification;
            if (classification is HourlyClassification)
            {
                classificationCode = "hourly";
                var hourlyClassification = classification as HourlyClassification;
                insertClassificationCommand = CreateInsertHourlyClassificationCommand(hourlyClassification, employee);
            }
            else if (classification is SalariedClassification)
            {
                classificationCode = "salary";
                var salariedClassification = classification as SalariedClassification;
                insertClassificationCommand = CreateInsertSalariedClassificationCommand(salariedClassification, employee);
            }
            else if (classification is CommissionClassification)
            {
                classificationCode = "commission";
                var commissionClassification = classification as CommissionClassification;
                insertClassificationCommand = CreateInsertCommissionClassificationCommand(commissionClassification, employee);
            }
            else
            {
                classificationCode = "unknown";
            }
        }

        private SqlCommand CreateInsertHourlyClassificationCommand(HourlyClassification classification, Employee employee)
        {
            var sql = "insert into HourlyClassification values (@HourlyRate, @EmpId)";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@HourlyRate", classification.HourlyRate);
            command.Parameters.AddWithValue("@EmpId", employee.EmpId);
            return command;
        }

        private SqlCommand CreateInsertSalariedClassificationCommand(SalariedClassification classification, Employee employee)
        {
            var sql = "insert into SalariedClassification values (@Salary, @EmpId)";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@Salary", classification.Salary);
            command.Parameters.AddWithValue("@EmpId", employee.EmpId);
            return command;
        }

        private SqlCommand CreateInsertCommissionClassificationCommand(CommissionClassification classification, Employee employee)
        {
            var sql = "insert into CommissionedClassification values (@Salary, @Commission, @EmpId)";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@Salary", classification.BaseRate);
            command.Parameters.AddWithValue("@Commission", classification.CommissionRate);
            command.Parameters.AddWithValue("@EmpId", employee.EmpId);
            return command;
        }

        private static string ScheduleCode(PaymentSchedule schedule)
        {
            if (schedule is MonthlySchedule)
            {
                return "monthly";
            }

            if (schedule is WeeklySchedule)
            {
                return "weekly";
            }

            if (schedule is BiWeeklySchedule)
            {
                return "biweekly";
            }

            return "unknown";
        }
    }
}