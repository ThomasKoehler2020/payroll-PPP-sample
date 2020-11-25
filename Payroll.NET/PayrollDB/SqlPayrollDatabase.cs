using System;
using System.Collections;
using System.Data.SqlClient;
using Payroll;

namespace PayrollDB
{
    public class SqlPayrollDatabase : PayrollDatabase
    {
        private readonly SqlConnection connection;

        public SqlPayrollDatabase()
        {
            connection = new SqlConnection(@"Server=.\SQLEXPRESS;Database=Payroll;Trusted_Connection=True");
            connection.Open();
        }

        ~SqlPayrollDatabase()
        {
            connection.Close();
        }

        public void AddEmployee(Employee employee)
        {
            var operation = new SaveEmployeeOperation(employee, connection);
            operation.Execute();
        }

        public Employee GetEmployee(int id)
        {
            var loadOperation = new LoadEmployeeOperation(id, connection);
            loadOperation.Execute();
            return loadOperation.Employee;
        }

        public void DeleteEmployee(int id)
        {
            throw new NotImplementedException();
        }

        public void AddUnionMember(int id, Employee e)
        {
            throw new NotImplementedException();
        }

        public Employee GetUnionMember(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveUnionMember(int memberId)
        {
            throw new NotImplementedException();
        }

        public ArrayList GetAllEmployeeIds()
        {
            throw new NotImplementedException();
        }

        public IList GetAllEmployees()
        {
            throw new NotImplementedException();
        }
    }
}