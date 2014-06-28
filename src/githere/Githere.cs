using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LibGit2Sharp;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace githere
{
    /// <summary>
    /// A class detailing the margin's visual definition including both size and content.
    /// </summary>
    internal class githere : Canvas, IWpfTextViewMargin
    {
        public const string MarginName = "githere";
        private readonly Repository repo;
        private readonly Label statusLabel;
        private bool _isDisposed;

        /// <summary>
        /// Creates a <see cref="githere"/> for a given <see cref="IWpfTextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public githere(IWpfTextView textView)
        {
            Height = 20;
            ClipToBounds = true;
            Background = new SolidColorBrush(Colors.LightGreen);
            statusLabel = new Label {Background = new SolidColorBrush(Colors.LightGreen)};
            Children.Add(statusLabel);

            repo = GetRepo(textView);
            if (repo == null)
            {
                Visibility = Visibility.Collapsed;
                return;
            }
            var timer = new DispatcherTimer();
            timer.Tick += (o, e) => statusLabel.Content = GetRepoStatus();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        /// <summary>
        /// The <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation
        /// of the margin.
        /// </summary>
        public FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            // Since this is a horizontal margin, its width will be bound to the width of the text view.
            // Therefore, its size is its height.
            get
            {
                ThrowIfDisposed();
                return ActualHeight;
            }
        }

        public bool Enabled
        {
            // The margin should always be enabled
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        /// <summary>
        /// Returns an instance of the margin if this is the margin that has been requested.
        /// </summary>
        /// <param name="marginName">The name of the margin requested</param>
        /// <returns>An instance of githere or null</returns>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == MarginName) ? this : null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }

        private static Repository GetRepo(IWpfTextView textView)
        {
            ITextDocument document;
            textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof (ITextDocument), out document);
            var workingDir = Path.GetDirectoryName(document.FilePath);
            var repoDir = Repository.Discover(workingDir);
            return repoDir == null ? null : new Repository(repoDir);
        }

        private string GetRepoStatus()
        {
            var workingDirStatusString = FormatWorkingDirStatus(repo.Index.RetrieveStatus());
            return string.Format("[{0}{1}]", repo.Head.Name, workingDirStatusString);
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
            File.AppendAllText(@"c:\logs\githere.log", msg + "\r\n");
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }
    }
}