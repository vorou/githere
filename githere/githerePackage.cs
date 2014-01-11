using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
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
        private IVsStatusbar statusBar;
        private IVsStatusbar StatusBar
        {
            get
            {
                if (statusBar == null)
                    statusBar = GetService(typeof (SVsStatusbar)) as IVsStatusbar;

                return statusBar;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            AddMenuItem();
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
            WriteToStatusBar("privet!");
        }

        private void WriteToStatusBar(string privet)
        {
            int frozen;
            StatusBar.IsFrozen(out frozen);
            if (frozen == 0)
                StatusBar.SetText(privet);
        }
    }
}