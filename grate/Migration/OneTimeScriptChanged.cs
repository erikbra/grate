using System;

namespace grate.Migration;

public class OneTimeScriptChanged : Exception
{
    public OneTimeScriptChanged(string errorMessage) : base(errorMessage)
    {
    }
}
