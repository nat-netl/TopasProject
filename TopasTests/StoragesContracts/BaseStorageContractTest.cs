using TopasDatabase;
using TopasTests.Infrastructure;

namespace TopasTests.StoragesContracts;

internal abstract class BaseStorageContractTest
{
    protected TopasDbContext TopasDbContext { get; private set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        TopasDbContext = new TopasDbContext(new ConfigurationDatabaseTest());
        TopasDbContext.Database.EnsureDeleted();
        TopasDbContext.Database.EnsureCreated();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TopasDbContext.Database.EnsureDeleted();
        TopasDbContext.Dispose();
    }
}
