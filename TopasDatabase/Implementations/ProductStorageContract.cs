using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using TopasDatabase.Models;

namespace TopasDatabase.Implementations;

internal class ProductStorageContract : IProductStorageContract
{
    private readonly TopasDbContext _dbContext;
    private readonly Mapper _mapper;

    public ProductStorageContract(TopasDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDataModel>();
            cfg.CreateMap<ProductDataModel, Product>()
                .ForMember(x => x.IsDeleted, x => x.MapFrom(_ => false));
            cfg.CreateMap<ProductHistory, ProductHistoryDataModel>();
        });
        _mapper = new Mapper(config);
    }

    public List<ProductDataModel> GetList(bool onlyActive = true, string? manufacturerId = null)
    {
        try
        {
            var query = _dbContext.Products.AsQueryable();
            if (onlyActive)
                query = query.Where(x => !x.IsDeleted);
            if (manufacturerId is not null)
                query = query.Where(x => x.ManufacturerId == manufacturerId);
            return [.. query.Select(x => _mapper.Map<ProductDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<ProductHistoryDataModel> GetHistoryByProductId(string productId)
    {
        try
        {
            return [.. _dbContext.ProductHistories
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.ChangeDate)
                .Select(x => _mapper.Map<ProductHistoryDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ProductDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<ProductDataModel>(GetProductById(id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ProductDataModel? GetElementByName(string name)
    {
        try
        {
            return _mapper.Map<ProductDataModel>(_dbContext.Products.FirstOrDefault(x => x.ProductName == name && !x.IsDeleted));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void AddElement(ProductDataModel productDataModel)
    {
        try
        {
            _dbContext.Products.Add(_mapper.Map<Product>(productDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("Id", productDataModel.Id);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Products_ProductName_IsDeleted" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("ProductName", productDataModel.ProductName);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void UpdElement(ProductDataModel productDataModel)
    {
        try
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var element = GetProductById(productDataModel.Id) ?? throw new ElementNotFoundException(productDataModel.Id);
                if (element.Price != productDataModel.Price)
                {
                    _dbContext.ProductHistories.Add(new ProductHistory { ProductId = element.Id, OldPrice = element.Price });
                    _dbContext.SaveChanges();
                }
                _mapper.Map(productDataModel, element);
                _dbContext.Products.Update(element);
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Products_ProductName_IsDeleted" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("ProductName", productDataModel.ProductName);
        }
        catch (Exception ex) when (ex is ElementDeletedException or ElementNotFoundException)
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
            var element = GetProductById(id) ?? throw new ElementNotFoundException(id);
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

    private Product? GetProductById(string id) => _dbContext.Products.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
}
