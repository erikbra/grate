using System;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.Architecture;

namespace grate.unittests.TestInfrastructure;

public abstract class TestContextBase
{
    protected static string DockerPlatform() => RuntimeInformation.ProcessArchitecture switch
    {
        Arm64 => "linux/arm64",
        X64 => "linux/amd64",
        var other => throw new PlatformNotSupportedException("Unsupported platform for running tests: " + other)
    };

}
