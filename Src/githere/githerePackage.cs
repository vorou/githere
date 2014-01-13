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
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [Guid(GuidList.guidGitherePkgString)]
    public sealed class GitherePackage : Package
    {
        private DTE dte;
        private SolutionEvents solutionEvents;

        protected override void Initialize()
        {
            base.Initialize();
            dte = GetGlobalService(typeof (SDTE)) as DTE;
            solutionEvents = dte.Events.SolutionEvents;
            solutionEvents.Opened += OnSolutionOpened;
        }

        private void OnSolutionOpened()
        {
            var slnDir = Path.GetDirectoryName(dte.Solution.FullName);
            using (var repo = new Repository(new DirectoryInfo(slnDir).Parent.FullName))
            {
                var headName = repo.Head.Name;
                dte.StatusBar.Text = string.Format("[{0}]", headName);
            }
        }
    }
}