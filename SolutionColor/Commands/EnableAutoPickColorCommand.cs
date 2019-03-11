using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace SolutionColor.Commands
{
    /// <summary>
    /// Command enable/disable automatic color picking.
    /// </summary>
    internal sealed class EnableAutoPickColorCommand : Command<EnableAutoPickColorCommand>
    {
        private readonly MenuCommand menuItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableAutoPickColorCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private EnableAutoPickColorCommand(SolutionColorPackage package) : base(0x0102, package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (((IServiceProvider)package).GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(SolutionColorPackage.ToolbarCommandSetGuid, commandId);
                menuItem = new MenuCommand(Execute, menuCommandID);
                commandService.AddCommand(menuItem);

                menuItem.Checked = package.Settings.IsAutomaticColorPickEnabled();
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(SolutionColorPackage package)
        {
            new EnableAutoPickColorCommand(package);
        }

        private void Execute(object sender, EventArgs e)
        {
            menuItem.Checked = !menuItem.Checked;
            package.Settings.SetAutomaticColorPickEnabled(menuItem.Checked);
        }
    }
}