using TopasContracts.Infrastructure;

namespace TopasTests.Infrastructure;

internal class ConfigurationDatabaseTest : IConfigurationDatabase
{
    public string ConnectionString => "Host=localhost;Port=5433;Database=TopasTest;Username=postgres;Password=Filosof;";
}
