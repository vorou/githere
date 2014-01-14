﻿using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using LibGit2Sharp;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace vorou.githere
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [Guid(GuidList.guidGitherePkgString)]
    public sealed class GitherePackage : Package
    {
        private DocumentEvents documentEvents;
        private DTE dte;
        private SolutionEvents solutionEvents;

        protected override void Initialize()
        {
            base.Initialize();
            dte = GetGlobalService(typeof (SDTE)) as DTE;
            solutionEvents = dte.Events.SolutionEvents;
            solutionEvents.Opened += UpdateGitStatus;
            documentEvents = dte.Events.DocumentEvents;
            documentEvents.DocumentSaved += _ => UpdateGitStatus();
        }

        private void UpdateGitStatus()
        {
            var slnDir = Path.GetDirectoryName(dte.Solution.FullName);
            var statusBar = GetService(typeof (SVsStatusbar)) as IVsStatusbar;
            using (var repo = new Repository(new DirectoryInfo(slnDir).Parent.FullName))
            {
                statusBar.SetText(string.Format("[{0} ~{1}]", repo.Head.Name, repo.Index.RetrieveStatus().Modified.Count()));
                statusBar.FreezeOutput(1);
                Task.Delay(1000);
                statusBar.FreezeOutput(0);
            }
        }
    }
}