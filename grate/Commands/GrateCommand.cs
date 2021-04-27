using System.CommandLine;

namespace grate.Commands
{
    public abstract class GrateCommand: Command
    {
        protected GrateCommand(string name, string? description = null) : base(name, description)
        {
        }
    }
}