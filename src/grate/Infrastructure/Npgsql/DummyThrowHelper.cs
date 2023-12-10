namespace grate.Infrastructure.Npgsql;

public static class DummyThrowHelper
{
    public static void ThrowInvalidOperationException(string message, string paramName)
    {
        throw new InvalidOperationException(message) { Data = { { "ParamName", paramName } } };
    }

    public static void ThrowNotSupportedException(string message)
    {
        throw new NotSupportedException(message);
    }
}
