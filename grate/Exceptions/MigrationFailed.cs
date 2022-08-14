using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grate.Exceptions;

public class MigrationFailed: AggregateException
{
    private const string _message = "Migration failed due to errors";

    public MigrationFailed(IEnumerable<Exception> exceptions)
        : base(_message, exceptions)
    {
    }
    
    public MigrationFailed(params Exception[] exceptions)
        : base(_message, exceptions)
    { }

    public override string Message
    {
        get
        {
            if (!InnerExceptions.Any())
            {
                return base.Message;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(_message);
            sb.Append(":\n");
            for (int i = 0; i < InnerExceptions.Count; i++)
            {
                sb.Append(" * ");
                sb.Append(InnerExceptions[i].Message);
                sb.Append('\n');
            }
            sb.Length--;
            return sb.ToString();
        }
    }
}
