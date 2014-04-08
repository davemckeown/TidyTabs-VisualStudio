// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TidyTabsPackage.cs" company="Dave McKeown">
//   Apache 2.0 License
// </copyright>
// <summary>
//   The main Visual Studio package for the Tidy Tabs extension
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DaveMcKeown.TidyTabs
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;

    using DaveMcKeown.TidyTabs.Properties;

    using EnvDTE;

    using EnvDTE80;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using Task = System.Threading.Tasks.Task;

    /// <summary>
    ///     The main Visual Studio package for the Tidy Tabs extension
    /// </summary>
    [ProvideOptionPage(typeof(TidyTabsOptionPage), "Tidy Tabs", "Options", 1000, 1001, false)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [Guid(GuidList.guidTidyTabsPkgString)]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
    public sealed class TidyTabsPackage : Package, IVsBroadcastMessageEvents, IDisposable
    {
        /// <summary>
        ///     A lock object for document purge operations
        /// </summary>
        private readonly object documentPurgeLock = new object();

        /// <summary>
        ///     Dictionary that tracks the path to a document and the last time it was viewed
        /// </summary>
        private readonly ConcurrentDictionary<string, DateTime> documentLastSeen = new ConcurrentDictionary<string, DateTime>();

        /// <summary>
        ///     Visual studio build events
        /// </summary>
        private BuildEvents buildEvents;

        /// <summary>
        ///     Disposed state
        /// </summary>
        private bool disposed;

        /// <summary>
        ///     Visual studio document events
        /// </summary>
        private DocumentEvents documentEvents;

        /// <summary>
        ///     The time of the last text editor action
        /// </summary>
        private DateTime lastAction = DateTime.MinValue;

        /// <summary>
        ///     Backing field for the service provider
        /// </summary>
        private ServiceProvider provider;

        /// <summary>
        ///     The visual studio shell reference
        /// </summary>
        private IVsShell shell;

        /// <summary>
        ///     Shell cookie from message subscription
        /// </summary>
        private uint shellCookie;

        /// <summary>
        ///     Visual studio solution events
        /// </summary>
        private SolutionEvents solutionEvents;

        /// <summary>
        ///     Visual studio text editor events
        /// </summary>
        private TextEditorEvents textEditorEvents;

        /// <summary>
        ///     The DTE COM object for the visual studio automation object
        /// </summary>
        private DTE2 visualStudio;

        /// <summary>
        ///     Visual studio window events
        /// </summary>
        private WindowEvents windowEvents;

        /// <summary>
        ///     Gets the Visual Studio service provider
        /// </summary>
        public IServiceProvider Provider
        {
            get
            {
                return provider ?? (provider = new ServiceProvider(VisualStudio.DTE as Microsoft.VisualStudio.OLE.Interop.IServiceProvider));
            }
        }

        /// <summary>
        ///     Gets the Visual Studio DTE COM Object
        /// </summary>
        public DTE2 VisualStudio
        {
            get
            {
                return visualStudio ?? (visualStudio = GetGlobalService(typeof(SDTE)) as DTE2);
            }
        }

        /// <summary>
        ///     Gets the settings for Tidy Tabs
        /// </summary>
        internal Settings Settings
        {
            get
            {
                return SettingsProvider.Instance;
            }
        }

        /// <summary>
        ///     Implements IDisposable
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                if (provider != null)
                {
                    provider.Dispose();
                }

                if (shell != null)
                {
                    shell.UnadviseBroadcastMessages(shellCookie);
                }

                disposed = true;
            }
        }

        /// <summary>Implements the IVsBroadcastMessageEvents interface</summary>
        /// <param name="msg">The notification message</param>
        /// <param name="wordParam">Word value parameter</param>
        /// <param name="longParam">Long integer parameter</param>
        /// <returns>S_OK on success</returns>
        public int OnBroadcastMessage(uint msg, IntPtr wordParam, IntPtr longParam)
        {
            // WM_ACTIVATEAPP 
            if (msg == 0x1C)
            {
                if (lastAction != DateTime.MinValue)
                {
                    TimeSpan idleTime = DateTime.Now - lastAction;

                    foreach (string path in documentLastSeen.Keys)
                    {
                        documentLastSeen[path] = documentLastSeen[path].Add(idleTime);
                    }
                }

                lastAction = DateTime.Now;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            try
            {
                if (VisualStudio.Solution != null)
                {
                    Log.Message("Starting Tidy Tabs inside Visual Studio {0} {1} for solution {2}", VisualStudio.Edition, VisualStudio.Version, VisualStudio.Solution.FullName);
                }

                windowEvents = VisualStudio.Events.WindowEvents;
                documentEvents = VisualStudio.Events.DocumentEvents;
                textEditorEvents = VisualStudio.Events.TextEditorEvents;
                solutionEvents = VisualStudio.Events.SolutionEvents;
                buildEvents = VisualStudio.Events.BuildEvents;

                windowEvents.WindowActivated += WindowEventsWindowActivated;
                documentEvents.DocumentClosing += DocumentEventsOnDocumentClosing;
                documentEvents.DocumentSaved += DocumentEventsOnDocumentSaved;
                textEditorEvents.LineChanged += TextEditorEventsOnLineChanged;
                solutionEvents.Opened += SolutionEventsOnOpened;
                buildEvents.OnBuildBegin += BuildEventsOnOnBuildBegin;

                shell = (IVsShell)GetService(typeof(SVsShell));

                if (shell != null)
                {
                    shell.AdviseBroadcastMessages(this, out shellCookie);
                }

                OleMenuCommandService menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

                if (menuCommandService != null)
                {
                    CommandID menuCommandId = new CommandID(GuidList.guidTidyTabsCmdSet, (int)PkgCmdIDList.cmdidTidyTabs);
                    MenuCommand menuItem = new MenuCommand(TidyTabsMenuItemCommandActivated, menuCommandId);
                    menuCommandService.AddCommand(menuItem);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>Closes stale windows when a build is triggered</summary>
        /// <param name="scope">The build scope</param>
        /// <param name="action">The build action</param>
        private void BuildEventsOnOnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            try
            {
                lastAction = DateTime.Now;

                Task.Factory.StartNew(() => TidyTabs(true));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>The cleanup action for Tidy Tabs that closes stale windows and document windows beyond the max open
        ///     window threshold</summary>
        /// <param name="autoSaveTriggered">Flag that indicates if the action was triggered by a document save event</param>
        private void TidyTabs(bool autoSaveTriggered)
        {
            if (autoSaveTriggered && !Settings.PurgeStaleTabsOnSave)
            {
                return;
            }

            lock (documentPurgeLock)
            {
                CloseStaleWindows();
                CloseOldestWindows();
            }
        }

        /// <summary>
        /// Closes a window if it is saved, not active, and not pinned
        /// </summary>
        /// <param name="window">The document window</param>
        /// <returns>True if window was closed</returns>
        private bool CloseDocumentWindow(Window window)
        {
            if (window != VisualStudio.ActiveWindow && window.Document.Saved && !Provider.IsWindowPinned(window.Document.FullName))
            {
                window.Close();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Closes stale windows that haven't been viewed recently
        /// </summary>
        private void CloseStaleWindows()
        {
            Dictionary<string, Window> allWindows = VisualStudio.Windows.GetDocumentWindows().ToDictionary(x => x.Document.FullName);
            int closeMaxTabs = allWindows.Count - Settings.TabCloseThreshold;
            List<DocumentTimestamp> inactiveWindows = documentLastSeen.GetInactiveTabKeys().ToList();
            int closedTabsCtr = 0;

            foreach (DocumentTimestamp tab in inactiveWindows.Where(x => allWindows.ContainsKey(x.DocumentPath)))
            {
                Window window = allWindows[tab.DocumentPath];

                if (CloseDocumentWindow(window))
                {
                    closedTabsCtr++;
                }

                if (closedTabsCtr >= closeMaxTabs)
                {
                    break;
                }
            }

            if (closedTabsCtr > 0)
            {
                Log.Message("Closed {0} tabs that were inactive for longer than {1} minutes", closedTabsCtr, Settings.TabTimeoutMinutes);
            }
        }

        /// <summary>
        ///     Close the oldest windows to keep the maximum open document tab count at threshold
        /// </summary>
        private void CloseOldestWindows()
        {
            Dictionary<string, Window> allWindows = VisualStudio.Windows.GetDocumentWindows().ToDictionary(x => x.Document.FullName);

            int startingWindowCount = allWindows.Count;
            int documentWindows = startingWindowCount;

            foreach (string documentPath in documentLastSeen.OrderBy(x => x.Value).Select(x => x.Key))
            {
                if (documentWindows <= Settings.MaxOpenTabs)
                {
                    break;
                }

                if (CloseDocumentWindow(allWindows[documentPath]))
                {
                    documentWindows--;
                }
            }

            if (documentWindows != startingWindowCount)
            {
                Log.Message("Closed {0} tabs to maintain a max open document count of {1}", startingWindowCount - documentWindows, Settings.MaxOpenTabs);
            }
        }

        /// <summary>Removes the document from the last seen cache when it is being closed</summary>
        /// <param name="document">The document being closed</param>
        private void DocumentEventsOnDocumentClosing(Document document)
        {
            try
            {
                lastAction = DateTime.Now;

                if (document == null)
                {
                    return;
                }

                DateTime value;
                documentLastSeen.TryRemove(document.FullName, out value);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>Closes stale windows when a document is saved</summary>
        /// <param name="document">The document being saved</param>
        private void DocumentEventsOnDocumentSaved(Document document)
        {
            try
            {
                lastAction = DateTime.Now;
                Task.Factory.StartNew(() => TidyTabs(true));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        ///     Solution opened event handler initializes the document last seen collection for previously open documents
        /// </summary>
        private void SolutionEventsOnOpened()
        {
            try
            {
                foreach (Window window in VisualStudio.Windows.GetDocumentWindows())
                {
                    documentLastSeen.TryAdd(window.Document.FullName, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>Text editor line changed event handler</summary>
        /// <param name="startPoint">Starting text point</param>
        /// <param name="endPoint">Ending text point</param>
        /// <param name="hint">Hint value</param>
        private void TextEditorEventsOnLineChanged(TextPoint startPoint, TextPoint endPoint, int hint)
        {
            lastAction = DateTime.Now;
        }

        /// <summary>MenuItem command handler</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event arguments</param>
        private void TidyTabsMenuItemCommandActivated(object sender, EventArgs e)
        {
            try
            {
                Log.Message("Tidy Tabs keyboard shortcut triggered");

                Task.Factory.StartNew(() => TidyTabs(false));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>Updates the timestamp for a document path</summary>
        /// <param name="document">The document to update</param>
        private void UpdateDocumentTimestamp(Document document)
        {
            if (document == null)
            {
                return;
            }

            if (!documentLastSeen.ContainsKey(document.FullName))
            {
                documentLastSeen.TryAdd(document.FullName, DateTime.Now);
            }
            else
            {
                documentLastSeen[document.FullName] = DateTime.Now;
            }
        }

        /// <summary>Updates the timestamp on the window that is being opened as well as the one losing focus</summary>
        /// <param name="gotFocus">The window gaining focus</param>
        /// <param name="lostFocus">The window losing focus</param>
        private void WindowEventsWindowActivated(Window gotFocus, Window lostFocus)
        {
            try
            {
                lastAction = DateTime.Now;
                UpdateDocumentTimestamp(gotFocus.Document);

                if (lostFocus != null)
                {
                    UpdateDocumentTimestamp(lostFocus.Document);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}