﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibGit2Sharp;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace githere
{
    internal class githere : Canvas, IWpfTextViewMargin
    {
        public const string MarginName = "githere";
        private static Repository repo;
        private static FileSystemWatcher HeadWatcher;
        private readonly Label statusLabel;
        private bool _isDisposed;

        public githere(IWpfTextView textView)
        {
            Log("githere ctor called");
            Height = 20;
            ClipToBounds = true;
            statusLabel = new Label {FontFamily = new FontFamily("Consolas"), Foreground = new SolidColorBrush(Colors.DarkGray)};
            Children.Add(statusLabel);

            if (repo == null)
                repo = GetRepo(textView);
            if (repo == null)
            {
                Log("repo wasn't found");
                Visibility = Visibility.Collapsed;
                return;
            }
            Log("repo was found");
            if (HeadWatcher == null)
            {
                var headWatcher = new FileSystemWatcher(repo.Info.Path) {Filter = "HEAD"};
                Log("filesystemwatcher created");
                headWatcher.Changed += (o, e) =>
                                       {
                                           Log("HEAD changed, trying to read status");
                                           string status;
                                           try
                                           {
                                               status = GetRepoStatus();
                                           }
                                           catch (Exception exception)
                                           {
                                               status = exception.Message;
                                               Log(exception.Message + " " + exception.ToString());
                                           }
                                           Log("status: " + status);
                                           statusLabel.Content = status;
                                       };
                Log("filesystemwatcher subscribed to");
                headWatcher.EnableRaisingEvents = true;
                Log("filesystemwatcher enabled");
                HeadWatcher = headWatcher;
            }
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return ActualHeight;
            }
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == MarginName) ? this : null;
        }

        public void Dispose()
        {
            Log("disposing");
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
            Log("invoked GetRepoStatus()");
            var workingDirStatusString = FormatWorkingDirStatus(repo.RetrieveStatus());
            var repoStatus = $"[{repo.Head.FriendlyName}{workingDirStatusString}]";
            Log("repo status was read: " + repoStatus);
            return repoStatus;
        }

        private static string FormatWorkingDirStatus(RepositoryStatus index)
        {
            var untracked = index.Untracked.Count();
            var modified = index.Modified.Count();
            var missing = index.Missing.Count();
            if (untracked == 0 && modified == 0 && missing == 0)
                return "";

            return $" +{untracked} ~{modified} -{missing}";
        }

        [Conditional("DEBUG")]
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