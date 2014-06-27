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
        private const string LogFile = @"C:\logs\githere.log";
        private DTE dte;
        private string repoDir;
        private SolutionEvents solutionEvents;
        private StatusBarService statusBarService;
        private DispatcherTimer timer;

        protected override void Initialize()
        {
            base.Initialize();
            if (File.Exists(LogFile))
                File.Delete(LogFile);
            Log("Initialize was called.");
            dte = GetGlobalService(typeof (SDTE)) as DTE;
            statusBarService = new StatusBarService(GetService(typeof (SVsStatusbar)) as IVsStatusbar);
            solutionEvents = dte.Events.SolutionEvents;
            solutionEvents.BeforeClosing += StopStatusUpdater;
            solutionEvents.Opened += StartStatusUpdater;
        }

        private void StartStatusUpdater()
        {
            repoDir = GetCurrentRepoDir();
            if (repoDir == null)
                return;
            timer = new DispatcherTimer();
            timer.Tick += (o, e) => UpdateGitStatus();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
            Log("Status updating started.");
        }

        private void StopStatusUpdater()
        {
            timer.Stop();
            Log("Status updating stopped.");
        }

        private void UpdateGitStatus()
        {
            using (var repo = new Repository(repoDir))
            {
                var workingDirStatusString = FormatWorkingDirStatus(repo.Index.RetrieveStatus());
                var statusString = string.Format("[{0}{1}]", repo.Head.Name, workingDirStatusString);
                statusBarService.SetText(statusString);
                Log("Statusbar updated to " + statusString);
            }
        }

        private string GetCurrentRepoDir()
        {
            Log("Locating repository...");
            var slnDir = Path.GetDirectoryName(dte.Solution.FullName);
            Log("Starting from " + slnDir);
            var repoDir = GetRepoDir(slnDir);
            if (repoDir != null)
                Log("Found repository at " + repoDir);
            else
                Log("Failed to locate repository!");
            return repoDir;
        }

        private static string GetRepoDir(string slnDir)
        {
            var current = slnDir;
            while (true)
            {
                if (Repository.IsValid(current))
                    return current;
                var parent = new DirectoryInfo(current).Parent;
                if (parent == null)
                    return null;
                current = parent.FullName;
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

        private static void Log(string msg)
        {
            File.AppendAllText(LogFile, msg + "\r\n");
        }
    }
}