namespace SolutionColor.Commands
{
    internal abstract class Command
    {
        public readonly int CommandId;
        protected readonly SolutionColorPackage package;

        protected Command(int commandId, SolutionColorPackage package)
        {
            CommandId = commandId;
            this.package = package;
        }
    }
}