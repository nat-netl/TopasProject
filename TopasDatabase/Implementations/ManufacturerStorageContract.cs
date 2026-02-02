using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using TopasDatabase.Models;

namespace TopasDatabase.Implementations;

internal class ManufacturerStorageContract : IManufacturerStorageContract
{
    private readonly TopasDbContext _dbContext;
    private readonly Mapper _mapper;

    public ManufacturerStorageContract(TopasDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Manufacturer, ManufacturerDataModel>();
            cfg.CreateMap<ManufacturerDataModel, Manufacturer>();
        });
        _mapper = new Mapper(config);
    }

    public List<ManufacturerDataModel> GetList()
    {
        try
        {
            return [.. _dbContext.Manufacturers.Select(x => _mapper.Map<ManufacturerDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ManufacturerDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<ManufacturerDataModel>(GetManufacturerById(id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ManufacturerDataModel? GetElementByName(string name)
    {
        try
        {
            return _mapper.Map<ManufacturerDataModel>(_dbContext.Manufacturers.FirstOrDefault(x => x.ManufacturerName == name));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ManufacturerDataModel? GetElementByOldName(string name)
    {
        try
        {
            return _mapper.Map<ManufacturerDataModel>(_dbContext.Manufacturers.FirstOrDefault(x =>
                x.PrevManufacturerName == name || x.PrevPrevManufacturerName == name));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void AddElement(ManufacturerDataModel manufacturerDataModel)
    {
        try
        {
            _dbContext.Manufacturers.Add(_mapper.Map<Manufacturer>(manufacturerDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("Id", manufacturerDataModel.Id);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Manufacturers_ManufacturerName" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("ManufacturerName", manufacturerDataModel.ManufacturerName);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void UpdElement(ManufacturerDataModel manufacturerDataModel)
    {
        try
        {
            var element = GetManufacturerById(manufacturerDataModel.Id) ?? throw new ElementNotFoundException(manufacturerDataModel.Id);
            if (element.ManufacturerName != manufacturerDataModel.ManufacturerName)
            {
                element.PrevPrevManufacturerName = element.PrevManufacturerName;
                element.PrevManufacturerName = element.ManufacturerName;
                element.ManufacturerName = manufacturerDataModel.ManufacturerName;
            }
            _dbContext.Manufacturers.Update(element);
            _dbContext.SaveChanges();
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Manufacturers_ManufacturerName" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("ManufacturerName", manufacturerDataModel.ManufacturerName);
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
            var element = GetManufacturerById(id) ?? throw new ElementNotFoundException(id);
            _dbContext.Manufacturers.Remove(element);
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

    private Manufacturer? GetManufacturerById(string id) => _dbContext.Manufacturers.FirstOrDefault(x => x.Id == id);
}
