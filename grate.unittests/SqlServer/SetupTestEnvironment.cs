﻿using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer
{
    [SetUpFixture]
    [Category("SqlServer")]
    public class SetupTestEnvironment : Generic.GenericSetupTestEnvironment
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServer;
    }
}