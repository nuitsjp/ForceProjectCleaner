//------------------------------------------------------------------------------
// <copyright file="ForceCleanSolutionCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace ForceProjectCleaner
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ForceCleanSolutionCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("8ee22e1b-61fb-42b7-93eb-010d9630bc37");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        private readonly MenuCommand _menuItem;

        private OutputWindow _outputWindow;
        /// <summary>
        /// Initializes a new instance of the <see cref="ForceCleanSolutionCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ForceCleanSolutionCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandId = new CommandID(CommandSet, CommandId);
                _menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
                commandService.AddCommand(_menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ForceCleanSolutionCommand Instance
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
            Instance = new ForceCleanSolutionCommand(package);
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
                var dte = (DTE) Package.GetGlobalService(typeof(DTE));
                var solution = dte.Solution;
                for (int i = 0; i < solution.Projects.Count; i++)
                {
                    var project = solution.Projects.Item(i + 1);
                    WriteLog($"Project Name:{project.Name} FullName:{project.FullName}");
                    if (!string.IsNullOrWhiteSpace(project.FullName))
                    {
                        var projectFile = new FileInfo(project.FullName);
                        var projectDirectory = projectFile.Directory;
                        // ReSharper disable once PossibleNullReferenceException
                        ForceDeleteDirectory(Path.Combine(projectDirectory.FullName, "bin"));
                        ForceDeleteDirectory(Path.Combine(projectDirectory.FullName, "obj"));
                    }
                }
            }
            catch (NotImplementedException)
            {
                // 開かれていないとき発生する？
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
        }

        private void ForceDeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            try
            {
                WriteLog($"Delete {path}");
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
        }

        private void WriteLog(Exception e)
        {
            WriteLog($"Exception:{e.GetType().FullName} Message:{e.Message} StackTrace:{e.StackTrace}");
        }

        private void WriteLog(string message)
        {
            if (_outputWindow == null)
            {
                var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                Window window = dte.Windows.Item(Constants.vsWindowKindOutput);
                _outputWindow = (OutputWindow)window.Object;
            }
            _outputWindow.ActivePane.Activate();
            _outputWindow.ActivePane.OutputString($"ForceProjectCleaner {message}{Environment.NewLine}");
        }
    }
}
