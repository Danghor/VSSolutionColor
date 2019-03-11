namespace SolutionColor.Commands
{
    internal abstract class Command<T>
    {
        protected readonly int commandId;
        protected readonly SolutionColorPackage package;

        protected Command(int commandId, SolutionColorPackage package)
        {
            this.commandId = commandId;
            this.package = package;
        }
    }
}