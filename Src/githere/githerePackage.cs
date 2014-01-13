using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using LibGit2Sharp;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace vorou.githere
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidGitherePkgString)]
    public sealed class GitherePackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            AddMenuItem();
        }

        private static string GetRepoDir()
        {
            var dte = GetGlobalService(typeof (SDTE)) as DTE;
            var slnDir = Path.GetDirectoryName(dte.Solution.FullName);
            return new DirectoryInfo(slnDir).Parent.FullName;
        }

        private void AddMenuItem()
        {
            var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null)
                return;

            var menuCommandId = new CommandID(GuidList.guidgithereCmdSet, (int) PkgCmdIDList.cmdidGithere);
            var menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
            mcs.AddCommand(menuItem);
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            WriteToStatusBar(GetCurrentBranchName(GetRepoDir()));
        }

        private static string GetCurrentBranchName(string repoDir)
        {
            using (var repo = new Repository(repoDir))
                return repo.Head.Name;
        }

        private void WriteToStatusBar(string privet)
        {
            var statusBar = GetService(typeof (SVsStatusbar)) as IVsStatusbar;
            int frozen;
            statusBar.IsFrozen(out frozen);
            if (frozen == 0)
                statusBar.SetText(privet);
        }
    }
}