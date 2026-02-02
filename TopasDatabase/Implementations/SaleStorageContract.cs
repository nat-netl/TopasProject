using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using TopasDatabase.Models;

namespace TopasDatabase.Implementations;

internal class SaleStorageContract : ISaleStorageContract
{
    private readonly TopasDbContext _dbContext;
    private readonly Mapper _mapper;

    public SaleStorageContract(TopasDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SaleProduct, SaleProductDataModel>();
            cfg.CreateMap<SaleProductDataModel, SaleProduct>();
            cfg.CreateMap<SaleDataModel, Sale>()
                .ForMember(x => x.IsCancel, x => x.MapFrom(_ => false))
                .ForMember(x => x.SaleProducts, x => x.MapFrom(src => src.Products));
        });
        _mapper = new Mapper(config);
    }

    private static SaleDataModel MapSaleToModel(Sale sale)
    {
        var products = (sale.SaleProducts ?? Enumerable.Empty<SaleProduct>())
            .Select(sp => new SaleProductDataModel(sp.SaleId, sp.ProductId, sp.Count, sp.Price))
            .ToList();
        return new SaleDataModel(sale.Id, sale.WorkerId, sale.BuyerId, sale.DiscountType, sale.IsCancel, products);
    }

    public List<SaleDataModel> GetList(DateTime? startDate = null, DateTime? endDate = null, string? workerId = null, string? buyerId = null, string? productId = null)
    {
        try
        {
            var query = _dbContext.Sales.Include(x => x.SaleProducts).AsQueryable();
            if (startDate is not null && endDate is not null)
                query = query.Where(x => x.SaleDate >= startDate && x.SaleDate < endDate);
            if (workerId is not null)
                query = query.Where(x => x.WorkerId == workerId);
            if (buyerId is not null)
                query = query.Where(x => x.BuyerId == buyerId);
            if (productId is not null)
                query = query.Where(x => x.SaleProducts!.Any(y => y.ProductId == productId));
            return [.. query.ToList().Select(MapSaleToModel)];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public SaleDataModel? GetElementById(string id)
    {
        try
        {
            var sale = GetSaleById(id);
            return sale is null ? null : MapSaleToModel(sale);
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void AddElement(SaleDataModel saleDataModel)
    {
        try
        {
            _dbContext.Sales.Add(_mapper.Map<Sale>(saleDataModel));
            _dbContext.SaveChanges();
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
            var element = GetSaleById(id) ?? throw new ElementNotFoundException(id);
            if (element.IsCancel)
                throw new ElementDeletedException(id);
            element.IsCancel = true;
            _dbContext.SaveChanges();
        }
        catch (ElementDeletedException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
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

    private Sale? GetSaleById(string id) => _dbContext.Sales.Include(x => x.SaleProducts).FirstOrDefault(x => x.Id == id);
}
