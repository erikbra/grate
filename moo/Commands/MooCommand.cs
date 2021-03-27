using System.CommandLine;

namespace moo.Commands
{
    public abstract class MooCommand: Command
    {
        protected MooCommand(string name, string? description = null) : base(name, description)
        {
        }
    }
}