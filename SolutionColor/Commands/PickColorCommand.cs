﻿using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;

namespace SolutionColor.Commands
{
    /// <summary>
    /// Command to open color picker in order to choose a color for the titlebar.
    /// </summary>
    internal sealed class PickColorCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickColorCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private PickColorCommand(SolutionColorPackage package) : base(0x0100, package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (((IServiceProvider)package).GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(SolutionColorPackage.ToolbarCommandSetGuid, CommandId);
                var menuItem = new MenuCommand(this.Execute, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static PickColorCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(SolutionColorPackage package)
        {
            Instance = new PickColorCommand(package);
        }

        private void Execute(object sender, EventArgs e)
        {
            var dialog = new ColorDialog
            {
                AllowFullOpen = true,
                Color = package.GetMainTitleBarColor(),
                CustomColors = package.Settings.GetCustomColorList()
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                package.SetTitleBarColor(dialog.Color);
                package.Settings.SaveOrOverwriteSolutionColor(VSUtils.GetCurrentSolutionPath(), dialog.Color);
            }

            package.Settings.SaveCustomColorList(dialog.CustomColors);
        }
    }
}