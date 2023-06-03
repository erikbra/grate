using System;

namespace grate.unittests.TestInfrastructure;

public class TestInfrastructureSetupException: Exception
{
    public TestInfrastructureSetupException(string message): base(message)
    {
    }
}
