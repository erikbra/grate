using FluentAssertions;
using grate.Infrastructure;

namespace Basic_tests.Infrastructure;


// ReSharper disable once InconsistentNaming
public class GrateEnvironment_
{
    [Fact]
    public void Always_runs_non_environment_files()
    {
        var env = new GrateEnvironment("TEST");
        var file = FullPath("just_a_normal_file.sql");

        env.ShouldRun(file).Should().BeTrue();
    }

    [Fact]
    public void Detects_environment_marker_in_start_of_filename()
    {
        var env = new GrateEnvironment("BOOYA");
        var file = FullPath("BOOYA.a_file.ENV.sql");

        env.ShouldRun(file).Should().BeTrue();
    }

    [Fact]
    public void Detects_environment_marker_in_middle_of_filename()
    {
        var env = new GrateEnvironment("BOOYA");
        var file = FullPath("a_file.BOOYA.ENV.with_middle.sql");

        env.ShouldRun(file).Should().BeTrue();
    }

    [Fact]
    public void Detects_environment_marker_in_end_of_filename()
    {
        var env = new GrateEnvironment("BOOYA");
        var file = FullPath("a_file..ENV.with_middle.BOOYA.sql");

        env.ShouldRun(file).Should().BeTrue();
    }
    
    [Fact]
    public void Supports_multiple_environments()
    {
        var env = new GrateEnvironment("BOOYA", "FOOYA");
        
        var file1 = FullPath("a_file.ENV.with.BOOYA.sql");
        var file2 = FullPath("a_file.ENV.with.FOOYA.sql");
        env.ShouldRun(file1).Should().BeTrue();
        env.ShouldRun(file2).Should().BeTrue();
    }

    [Fact]
    public void Does_not_run_for_other_environments()
    {
        var env = new GrateEnvironment("FOOFOO");
        var file = FullPath("BOOYA.a_file.ENV.sql");

        env.ShouldRun(file).Should().BeFalse();
    }

    private static string FullPath(string fileName)
    {
        return Path.Combine(Path.GetTempPath(), fileName);
    }

}
