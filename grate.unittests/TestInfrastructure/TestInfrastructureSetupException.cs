using System;

namespace Unit_tests.TestInfrastructure;

public class TestInfrastructureSetupException: Exception
{
    public TestInfrastructureSetupException(string message): base(message)
    {
    }
}
