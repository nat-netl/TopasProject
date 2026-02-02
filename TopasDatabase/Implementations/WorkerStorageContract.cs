using AutoMapper;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using TopasDatabase.Models;

namespace TopasDatabase.Implementations;

internal class WorkerStorageContract : IWorkerStorageContract
{
    private readonly TopasDbContext _dbContext;
    private readonly Mapper _mapper;

    public WorkerStorageContract(TopasDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Worker, WorkerDataModel>();
            cfg.CreateMap<WorkerDataModel, Worker>();
        });
        _mapper = new Mapper(config);
    }

    public List<WorkerDataModel> GetList(bool onlyActive = true, string? postId = null, DateTime? fromBirthDate = null, DateTime? toBirthDate = null, DateTime? fromEmploymentDate = null, DateTime? toEmploymentDate = null)
    {
        try
        {
            var query = _dbContext.Workers.AsQueryable();
            if (onlyActive)
                query = query.Where(x => !x.IsDeleted);
            if (postId is not null)
                query = query.Where(x => x.PostId == postId);
            if (fromBirthDate is not null && toBirthDate is not null)
                query = query.Where(x => x.BirthDate >= fromBirthDate && x.BirthDate <= toBirthDate);
            if (fromEmploymentDate is not null && toEmploymentDate is not null)
                query = query.Where(x => x.EmploymentDate >= fromEmploymentDate && x.EmploymentDate <= toEmploymentDate);
            return [.. query.Select(x => _mapper.Map<WorkerDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public WorkerDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<WorkerDataModel>(GetWorkerById(id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public WorkerDataModel? GetElementByFIO(string fio)
    {
        try
        {
            return _mapper.Map<WorkerDataModel>(_dbContext.Workers.FirstOrDefault(x => x.FIO == fio));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void AddElement(WorkerDataModel workerDataModel)
    {
        try
        {
            _dbContext.Workers.Add(_mapper.Map<Worker>(workerDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("Id", workerDataModel.Id);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void UpdElement(WorkerDataModel workerDataModel)
    {
        try
        {
            var element = GetWorkerById(workerDataModel.Id) ?? throw new ElementNotFoundException(workerDataModel.Id);
            _mapper.Map(workerDataModel, element);
            _dbContext.Workers.Update(element);
            _dbContext.SaveChanges();
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void DelElement(string id)
    {
        try
        {
            var element = GetWorkerById(id) ?? throw new ElementNotFoundException(id);
            element.IsDeleted = true;
            _dbContext.SaveChanges();
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    private Worker? GetWorkerById(string id) => _dbContext.Workers.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
}
