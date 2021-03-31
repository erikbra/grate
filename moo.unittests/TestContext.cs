namespace moo.unittests
{
    public static class MooTestContext
    {
        public static SqlServerTestContext SqlServer = new SqlServerTestContext();

    }

    public class SqlServerTestContext
    {
        public string AdminPassword { get; set; }
        public int Port { get; set; }
    }
}