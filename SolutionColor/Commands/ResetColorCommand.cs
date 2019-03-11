using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace SolutionColor.Commands
{
    /// <summary>
    /// Command to reset the title bar color.
    /// </summary>
    internal sealed class ResetColorCommand : Command<ResetColorCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResetColorCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ResetColorCommand(SolutionColorPackage package) : base(0x0101, package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (((IServiceProvider)package).GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(SolutionColorPackage.ToolbarCommandSetGuid, commandId);
                var menuItem = new MenuCommand(Execute, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(SolutionColorPackage package)
        {
            new ResetColorCommand(package);
        }

        private void Execute(object sender, EventArgs e)
        {
            package.ResetTitleBarColor();
            package.Settings.RemoveSolutionColorSetting(VSUtils.GetCurrentSolutionPath());
        }
    }
}