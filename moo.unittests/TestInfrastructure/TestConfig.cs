using System;

namespace moo.unittests.TestInfrastructure
{
    public static class TestConfig
    {
        private static readonly Random Random = new Random();

        public static string RandomDatabase() => Random.GetString(15);
    }
}