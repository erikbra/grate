namespace grate.unittests
{
    public static class GrateTestContext
    {
        public static SqlTestContext SqlServer = new();
        public static SqlTestContext Oracle = new();
        public static SqlTestContext PostgreSql = new();
    }

    public class SqlTestContext
    {
        public string? AdminPassword { get; set; }
        public int? Port { get; set; }
    }
   
}