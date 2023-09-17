using System;

namespace TestCommon.TestInfrastructure;

public class TestInfrastructureSetupException: Exception
{
    public TestInfrastructureSetupException(string message): base(message)
    {
    }
}
