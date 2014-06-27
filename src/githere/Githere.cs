using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;

namespace githere
{
    /// <summary>
    /// A class detailing the margin's visual definition including both size and content.
    /// </summary>
    internal class githere : Canvas, IWpfTextViewMargin
    {
        public const string MarginName = "githere";
        private bool _isDisposed = false;

        /// <summary>
        /// Creates a <see cref="githere"/> for a given <see cref="IWpfTextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public githere(IWpfTextView textView)
        {
            this.Height = 20;
            this.ClipToBounds = true;
            this.Background = new SolidColorBrush(Colors.LightGreen);

            // Add a green colored label that says "Hello World!"
            var label = new Label();
            label.Background = new SolidColorBrush(Colors.LightGreen);
            label.Content = "Hello World!";
            this.Children.Add(label);
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
                return this.ActualHeight;
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
            return (marginName == MarginName) ? (IWpfTextViewMargin) this : null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }
    }
}