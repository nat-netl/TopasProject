using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using TopasDatabase.Models;

namespace TopasDatabase.Implementations;

internal class BuyerStorageContract : IBuyerStorageContract
{
    private readonly TopasDbContext _dbContext;
    private readonly Mapper _mapper;

    public BuyerStorageContract(TopasDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Buyer, BuyerDataModel>();
            cfg.CreateMap<BuyerDataModel, Buyer>();
        });
        _mapper = new Mapper(config);
    }

    public List<BuyerDataModel> GetList()
    {
        try
        {
            return [.. _dbContext.Buyers.Select(x => _mapper.Map<BuyerDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public BuyerDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<BuyerDataModel>(GetBuyerById(id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public BuyerDataModel? GetElementByFIO(string fio)
    {
        try
        {
            return _mapper.Map<BuyerDataModel>(_dbContext.Buyers.FirstOrDefault(x => x.FIO == fio));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public BuyerDataModel? GetElementByPhoneNumber(string phoneNumber)
    {
        try
        {
            return _mapper.Map<BuyerDataModel>(_dbContext.Buyers.FirstOrDefault(x => x.PhoneNumber == phoneNumber));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void AddElement(BuyerDataModel buyerDataModel)
    {
        try
        {
            _dbContext.Buyers.Add(_mapper.Map<Buyer>(buyerDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("Id", buyerDataModel.Id);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Buyers_PhoneNumber" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("PhoneNumber", buyerDataModel.PhoneNumber);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void UpdElement(BuyerDataModel buyerDataModel)
    {
        try
        {
            var element = GetBuyerById(buyerDataModel.Id) ?? throw new ElementNotFoundException(buyerDataModel.Id);
            _mapper.Map(buyerDataModel, element);
            _dbContext.Buyers.Update(element);
            _dbContext.SaveChanges();
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Buyers_PhoneNumber" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("PhoneNumber", buyerDataModel.PhoneNumber);
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
            var element = GetBuyerById(id) ?? throw new ElementNotFoundException(id);
            _dbContext.Buyers.Remove(element);
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

    private Buyer? GetBuyerById(string id) => _dbContext.Buyers.FirstOrDefault(x => x.Id == id);
}
