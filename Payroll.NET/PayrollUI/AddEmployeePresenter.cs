using Payroll;

namespace PayrollUI
{
    public class AddEmployeePresenter
    {
        private string address;
        private double commission;
        private double commissionSalary;
        private readonly PayrollDatabase database;

        private int empId;
        private double hourlyRate;
        private bool isCommission;
        private bool isHourly;
        private bool isSalary;
        private string name;
        private double salary;
        private readonly AddEmployeeView view;

        public int EmpId
        {
            get => empId;
            set
            {
                empId = value;
                UpdateView();
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                UpdateView();
            }
        }

        public string Address
        {
            get => address;
            set
            {
                address = value;
                UpdateView();
            }
        }

        public bool IsHourly
        {
            get => isHourly;
            set
            {
                isHourly = value;
                UpdateView();
            }
        }

        public double HourlyRate
        {
            get => hourlyRate;
            set
            {
                hourlyRate = value;
                UpdateView();
            }
        }

        public bool IsSalary
        {
            get => isSalary;
            set
            {
                isSalary = value;
                UpdateView();
            }
        }

        public double Salary
        {
            get => salary;
            set
            {
                salary = value;
                UpdateView();
            }
        }

        public bool IsCommission
        {
            get => isCommission;
            set
            {
                isCommission = value;
                UpdateView();
            }
        }

        public double CommissionSalary
        {
            get => commissionSalary;
            set
            {
                commissionSalary = value;
                UpdateView();
            }
        }

        public double Commission
        {
            get => commission;
            set
            {
                commission = value;
                UpdateView();
            }
        }

        public TransactionContainer TransactionContainer { get; }

        public AddEmployeePresenter(AddEmployeeView view,
            TransactionContainer container,
            PayrollDatabase database)
        {
            this.view = view;
            TransactionContainer = container;
            this.database = database;
        }

        public bool AllInformationIsCollected()
        {
            var result = true;
            result &= empId > 0;
            result &= name != null && name.Length > 0;
            result &= address != null && address.Length > 0;
            result &= isHourly || isSalary || isCommission;
            if (isHourly)
            {
                result &= hourlyRate > 0;
            }
            else if (isSalary)
            {
                result &= salary > 0;
            }
            else if (isCommission)
            {
                result &= commission > 0;
                result &= commissionSalary > 0;
            }

            return result;
        }

        public virtual void AddEmployee()
        {
            TransactionContainer.Add(CreateTransaction());
        }

        public Transaction CreateTransaction()
        {
            if (isHourly)
            {
                return new AddHourlyEmployee(
                    empId, name, address, hourlyRate, database);
            }

            if (isSalary)
            {
                return new AddSalariedEmployee(
                    empId, name, address, salary, database);
            }

            return new AddCommissionedEmployee(
                empId, name, address, commissionSalary,
                commission, database);
        }

        private void UpdateView()
        {
            if (AllInformationIsCollected())
            {
                view.SubmitEnabled = true;
            }
            else
            {
                view.SubmitEnabled = false;
            }
        }
    }
}