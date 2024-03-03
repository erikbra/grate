namespace TestCommon.TestInfrastructure;

public interface ITestDatabase
{
    string AdminConnectionString { get; }
    string ConnectionString(string database);
    string UserConnectionString(string database);
}
