using System;

namespace grate.unittests.TestInfrastructure
{
    public static class TestConfig
    {
        private static readonly Random Random = new();

        public static string RandomDatabase() => Random.GetString(15);
    }
}
