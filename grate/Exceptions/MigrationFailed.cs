using System;
using System.Collections.Generic;

namespace grate.Exceptions;

public class MigrationFailed: AggregateException
{
    public MigrationFailed(IEnumerable<Exception> exceptions)
        : base("Migration failed due to errors", exceptions)
    { }
    
    public MigrationFailed(params Exception[] exceptions)
        : base("Migration failed due to errors", exceptions)
    { }
}
