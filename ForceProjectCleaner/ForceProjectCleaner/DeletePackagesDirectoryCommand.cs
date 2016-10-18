//------------------------------------------------------------------------------
// <copyright file="DeletePackagesDirectoryCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ForceProjectCleaner
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class DeletePackagesDirectoryCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x200;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        private readonly MenuCommand _menuItem;

        private OutputWindow _outputWindow;
        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePackagesDirectoryCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private DeletePackagesDirectoryCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(ForceCleanSolutionCommandPackage.CommandSet, CommandId);
                _menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(_menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DeletePackagesDirectoryCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        public bool Visible
        {
            get { return _menuItem.Visible; }
            set { _menuItem.Visible = value; }
        }

        public bool Enabled
        {
            get { return _menuItem.Enabled; }
            set { _menuItem.Enabled = value; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new DeletePackagesDirectoryCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                var solution = dte.Solution;
                Logger.Instance.WriteLog($"Solution FullName:{solution.FullName}");
                var solutionFile = new FileInfo(solution.FullName);
                var solutionDirectory = solutionFile.Directory;
                // ReSharper disable once PossibleNullReferenceException
                DirectoryDeleter.ForceDeleteDirectory(Path.Combine(solutionDirectory.FullName, "packages"));
            }
            catch (NotImplementedException)
            {
                // 開かれていないとき発生する？
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLog(ex);
            }
        }
    }
}
