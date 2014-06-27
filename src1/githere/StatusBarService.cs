using Microsoft.VisualStudio.Shell.Interop;

namespace vorou.githere
{
    public class StatusBarService
    {
        private readonly IVsStatusbar statusBar;

        public StatusBarService(IVsStatusbar statusBar)
        {
            this.statusBar = statusBar;
        }

        public void SetText(string text)
        {
            if (!IsFrozen)
            {
                statusBar.SetText(text);
            }
        }

        private bool IsFrozen
        {
            get
            {
                int frozen;
                statusBar.IsFrozen(out frozen);
                return frozen != 0;
            }
        }
    }
}