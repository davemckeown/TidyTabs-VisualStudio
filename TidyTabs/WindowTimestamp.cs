// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentTimestamp.cs" company="Dave McKeown">
//   Apache 2.0 License
// </copyright>
// <summary>
//   A more strongly typed key value pair that represents a document path and it's last view timestamp
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DaveMcKeown.TidyTabs
{
    using System;
    using EnvDTE;

    /// <summary>
    ///     A more strongly typed key value pair that represents a window hash code and it's last view timestamp
    /// </summary>
    internal class WindowTimestamp
    {
        /// <summary>Initializes a new instance of the WindowTimestamp class</summary>
        /// <param name="window">Hash code for a window</param>
        /// <param name="timestamp">The time it was last seen</param>
        public WindowTimestamp(Window window, DateTime timestamp)
        {
            Window = window;
            Timestamp = timestamp;
        }

        /// <summary>
        ///     Gets the Window
        /// </summary>
        public Window Window { get; private set; }

        /// <summary>
        ///     Gets the time a document was last seen
        /// </summary>
        public DateTime Timestamp { get; private set; }
    }
}