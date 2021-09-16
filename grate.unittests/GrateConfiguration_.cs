using FluentAssertions;
using grate.Configuration;
using NUnit.Framework;

namespace grate.unittests
{
    [TestFixture]
    [Category("Basic")]
    public class GrateConfiguration_
    {
        [Test]
        public void Uses_ConnectionString_with_master_db_if_adminConnectionString_is_not_set_Initial_Catalog()
        {
            var cfg = new GrateConfiguration()
                {ConnectionString = "Data source=Monomonojono;Initial Catalog=Øyenbryn;Lotsastuff"};

            cfg.AdminConnectionString.Should().Be("Data source=Monomonojono;Initial Catalog=master;Lotsastuff");
        }
        
        [Test]
        public void Uses_ConnectionString_with_master_db_if_adminConnectionString_is_not_set_Database()
        {
            var cfg = new GrateConfiguration()
                {ConnectionString = "Data source=Monomonojono;Database=Øyenbryn;Lotsastuff"};

            cfg.AdminConnectionString.Should().Be("Data source=Monomonojono;Database=master;Lotsastuff");
        }
        
    }
}
