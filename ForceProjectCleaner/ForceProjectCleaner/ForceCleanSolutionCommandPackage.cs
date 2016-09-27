//------------------------------------------------------------------------------
// <copyright file="ForceCleanSolutionCommandPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ForceProjectCleaner
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class ForceCleanSolutionCommandPackage : Package
    {
        /// <summary>
        /// ForceCleanSolutionCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "f70d5081-faaf-4ad5-9584-795b700e0c5d";

        /// <summary>
        /// Initializes a new instance of the <see cref="ForceCleanSolutionCommand"/> class.
        /// </summary>
        public ForceCleanSolutionCommandPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members
        private DTE _dte;
        private SolutionEvents _solutionEvents;
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            ForceCleanSolutionCommand.Initialize(this);
            _dte = GetService(typeof(SDTE)) as DTE;
            // ReSharper disable once PossibleNullReferenceException
            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.Opened += () =>
            {
                ForceCleanSolutionCommand.Instance.Visible = true;
            };
            _solutionEvents.BeforeClosing += () =>
            {
                ForceCleanSolutionCommand.Instance.Visible = false;
            };
            var buildEvents = _dte.Events.BuildEvents;
            buildEvents.OnBuildBegin += (scope, action) =>
            {
                ForceCleanSolutionCommand.Instance.Enabled = false;
            };
            buildEvents.OnBuildDone += (scope, action) =>
            {
                ForceCleanSolutionCommand.Instance.Enabled = true;
            };
            base.Initialize();
        }

        #endregion
    }
}
