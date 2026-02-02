using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.Infrastructure;

namespace TopasContracts.DataModels;

public class SalaryDataModel(string workerId, DateTime salaryDate, double workerSalary) : IValidation
{
    public string WorkerId { get; private set; } = workerId;
    public DateTime SalaryDate { get; private set; } = salaryDate;
    public double Salary { get; private set; } = workerSalary;

    public void Validate()
    {
        if (WorkerId.IsEmpty())
            throw new ValidationException("Field WorkerId is empty");
        if (!WorkerId.IsGuid())
            throw new ValidationException("The value in the field WorkerId is not a unique identifier");
        if (Salary <= 0)
            throw new ValidationException("Field Salary is less than or equal to 0");
    }
}
