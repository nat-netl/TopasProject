using TopasContracts.DataModels;

namespace TopasContracts.BusinessLogicsContracts;

public interface ISalaryBusinessLogicContract
{
    List<SalaryDataModel> GetAllSalariesByPeriod(DateTime fromDate, DateTime toDate);
    List<SalaryDataModel> GetAllSalariesByPeriodByWorker(DateTime fromDate, DateTime toDate, string workerId);
    void CalculateSalaryByMounth(DateTime date);
}
