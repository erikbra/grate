namespace grate.unittests
{
    public static class GrateTestContext
    {
        public static SqlServerTestContext SqlServer = new();
        public static OracleTestContext Oracle = new();
    }

    public class SqlServerTestContext
    {
        public string? AdminPassword { get; set; }
        public int? Port { get; set; }
    }
    
    public class OracleTestContext
    {
        public string? AdminPassword { get; set; }
        public int? Port { get; set; }
    }
}