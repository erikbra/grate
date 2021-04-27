namespace grate.unittests
{
    public static class GrateTestContext
    {
        public static SqlServerTestContext SqlServer = new SqlServerTestContext();

    }

    public class SqlServerTestContext
    {
        public string? AdminPassword { get; set; }
        public int? Port { get; set; }
    }
}