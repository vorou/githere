using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace githere
{
    [Export(typeof (IWpfTextViewMarginProvider))]
    [Name(githere.MarginName)]
    [Order(After = PredefinedMarginNames.HorizontalScrollBar)]
    [MarginContainer(PredefinedMarginNames.Bottom)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class MarginFactory : IWpfTextViewMarginProvider
    {
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            return new githere(textViewHost.TextView);
        }
    }
}