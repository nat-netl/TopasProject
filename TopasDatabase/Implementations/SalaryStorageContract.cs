using AutoMapper;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using TopasDatabase.Models;

namespace TopasDatabase.Implementations;

internal class SalaryStorageContract : ISalaryStorageContract
{
    private readonly TopasDbContext _dbContext;
    private readonly Mapper _mapper;

    public SalaryStorageContract(TopasDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Salary, SalaryDataModel>();
            cfg.CreateMap<SalaryDataModel, Salary>()
                .ForMember(dest => dest.WorkerSalary, opt => opt.MapFrom(src => src.Salary));
        });
        _mapper = new Mapper(config);
    }

    public List<SalaryDataModel> GetList(DateTime startDate, DateTime endDate, string? workerId = null)
    {
        try
        {
            var query = _dbContext.Salaries.Where(x => x.SalaryDate >= startDate && x.SalaryDate <= endDate);
            if (workerId is not null)
                query = query.Where(x => x.WorkerId == workerId);
            return [.. query.Select(x => _mapper.Map<SalaryDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void AddElement(SalaryDataModel salaryDataModel)
    {
        try
        {
            _dbContext.Salaries.Add(_mapper.Map<Salary>(salaryDataModel));
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }
}
