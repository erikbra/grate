using FluentAssertions;
using moo.Infrastructure;
using NUnit.Framework;

namespace moo.unittests.Infrastructure
{
    [TestFixture]
    public class MooEnvironment_
    {
        [Test]
        public void Always_runs_non_environment_files()
        {
            var env = new MooEnvironment("TEST");
            var file = "C:\\tmp\\just_a_normal_file.sql";

            env.ShouldRun(file).Should().BeTrue();
        }
        
        [Test]
        public void Detects_environment_marker_in_start_of_filename()
        {
            var env = new MooEnvironment("BOOYA");
            var file = "C:\\tmp\\BOOYA.a_file.ENV.sql";

            env.ShouldRun(file).Should().BeTrue();
        }
        
        [Test]
        public void Detects_environment_marker_in_middle_of_filename()
        {
            var env = new MooEnvironment("BOOYA");
            var file = "C:\\tmp\\a_file.BOOYA.ENV.with_middle.sql";

            env.ShouldRun(file).Should().BeTrue();
        }
        
        [Test]
        public void Detects_environment_marker_in_end_of_filename()
        {
            var env = new MooEnvironment("BOOYA");
            var file = "C:\\tmp\\a_file..ENV.with_middle.BOOYA.sql";

            env.ShouldRun(file).Should().BeTrue();
        }
        
        [Test]
        public void Does_not_run_for_other_environments()
        {
            var env = new MooEnvironment("FOOFOO");
            var file = "C:\\tmp\\BOOYA.a_file.ENV.sql";

            env.ShouldRun(file).Should().BeFalse();
        }
     
        
    }
}