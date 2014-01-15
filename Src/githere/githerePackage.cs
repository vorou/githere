using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Threading;
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
        private StatusBarService statusBarService;
        private DispatcherTimer timer;

        protected override void Initialize()
        {
            base.Initialize();
            dte = GetGlobalService(typeof (SDTE)) as DTE;
            statusBarService = new StatusBarService(GetService(typeof (SVsStatusbar)) as IVsStatusbar);
            StartTimer();
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += (o, e) => UpdateGitStatus();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void UpdateGitStatus()
        {
            var slnDir = Path.GetDirectoryName(dte.Solution.FullName);
            using (var repo = new Repository(new DirectoryInfo(slnDir).Parent.FullName))
            {
                var workingDirStatusString = FormatWorkingDirStatus(repo.Index.RetrieveStatus());
                var statusString = string.Format("[{0}{1}]", repo.Head.Name, workingDirStatusString);
                statusBarService.SetText(statusString);
            }
        }

        private static string FormatWorkingDirStatus(RepositoryStatus index)
        {
            var untracked = index.Untracked.Count();
            var modified = index.Modified.Count();
            var missing = index.Missing.Count();
            if (untracked == 0 && modified == 0 && missing == 0)
                return "";

            return string.Format(" +{0} ~{1} -{2}", untracked, modified, missing);
        }
    }
}