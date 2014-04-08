// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Dave McKeown">
//   Apache 2.0 License
// </copyright>
// <summary>
//   Extension methods used by Tidy Tabs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DaveMcKeown.TidyTabs
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using DaveMcKeown.TidyTabs.Properties;

    using EnvDTE;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    ///     Extension methods used by Tidy Tabs
    /// </summary>
    internal static class Extensions
    {
        /// <summary>Enumerates the DTE2 windows Property</summary>
        /// <param name="windows">The DTE2 windows Property</param>
        /// <returns>An enumerable sequence of windows</returns>
        public static IEnumerable<Window> GetDocumentWindows(this Windows windows)
        {
            return windows.Cast<Window>().Where(x => x.Document != null);
        }

        /// <summary>Enumerates the document tabs timeout dictionary and returns the key for tabs that are inactive</summary>
        /// <param name="documentTabKeys">Dictionary of document paths and last seen time stamps</param>
        /// <returns>Enumerable sequence of inactive tab keys</returns>
        public static IEnumerable<DocumentTimestamp> GetInactiveTabKeys(this ConcurrentDictionary<string, DateTime> documentTabKeys)
        {
            return
                documentTabKeys.Where(x => (DateTime.Now - x.Value) > TimeSpan.FromMinutes(Settings.Default.TabTimeoutMinutes))
                    .Select(x => new DocumentTimestamp(x.Key, x.Value))
                    .OrderByDescending(x => DateTime.Now - x.Timestamp);
        }

        /// <summary>Returns a IVsWindowFrame based on a document path</summary>
        /// <param name="provider">The IServiceProvider used to resolve the IVsWindowFrame</param>
        /// <param name="documentPath">The path to the document in the window</param>
        /// <returns>A IVsWindowFrame reference</returns>
        public static bool IsWindowPinned(this IServiceProvider provider, string documentPath)
        {
            IVsWindowFrame frame;
            IVsUIHierarchy uiHierarchy;
            uint item;

            VsShellUtilities.IsDocumentOpen(provider, documentPath, VSConstants.LOGVIEWID_Primary, out uiHierarchy, out item, out frame);

            return frame != null && frame.IsWindowPinned();
        }

        /// <summary>Checks to see if a IVsWindowFrame is pinned in the document well</summary>
        /// <param name="frame">The IVsWindowFrame reference</param>
        /// <returns>True if the window frame is pinned in the document well</returns>
        private static bool IsWindowPinned(this IVsWindowFrame frame)
        {
            object result;
            frame.GetProperty((int)__VSFPROPID5.VSFPROPID_IsPinned, out result);
            return Convert.ToBoolean(result);
        }
    }
}