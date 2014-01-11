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
        protected override void Initialize()
        {
            base.Initialize();

            var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                var menuCommandId = new CommandID(GuidList.guidgithereCmdSet, (int) PkgCmdIDList.cmdidGithere);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
                mcs.AddCommand(menuItem);
            }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            var outputWindow = GetGlobalService(typeof (SVsOutputWindow)) as IVsOutputWindow;

            var guidGeneral = VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            IVsOutputWindowPane pane;
            outputWindow.CreatePane(guidGeneral, "General", 1, 0);
            outputWindow.GetPane(guidGeneral, out pane);
            pane.Activate();
            pane.OutputString("Pandy!");

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
    }
}