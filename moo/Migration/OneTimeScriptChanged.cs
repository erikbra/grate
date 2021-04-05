using System;

namespace moo.Migration
{
    public class OneTimeScriptChanged : Exception
    {
        public OneTimeScriptChanged(string errorMessage) : base(errorMessage)
        {
        }
    }
}