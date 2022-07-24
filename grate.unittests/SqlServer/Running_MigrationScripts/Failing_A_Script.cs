using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
// ReSharper disable once InconsistentNaming
public class Failing_A_Script: Generic.Running_MigrationScripts.Failing_A_Script
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
    protected override string ExpectedErrorMessageForInvalidSql => "Incorrect syntax near 'TOP'.";
   
    
}
