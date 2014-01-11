using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace vorou.githere
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidgitherePkgString)]
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

            var uiShell = (IVsUIShell) GetService(typeof (SVsUIShell));
            var clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(0,
                                                               ref clsid,
                                                               "githere",
                                                               string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this),
                                                               string.Empty,
                                                               0,
                                                               OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                                               OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                                                               OLEMSGICON.OLEMSGICON_INFO,
                                                               0,
                                                               out result));
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