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

    /// <summary>
    ///     A more strongly typed key value pair that represents a document path and it's last view timestamp
    /// </summary>
    internal class DocumentTimestamp
    {
        /// <summary>Initializes a new instance of the DocumentTimestamp class</summary>
        /// <param name="path">Path to the document</param>
        /// <param name="timestamp">The time it was last seen</param>
        public DocumentTimestamp(string path, DateTime timestamp)
        {
            DocumentPath = path;
            Timestamp = timestamp;
        }

        /// <summary>
        ///     Gets the document path
        /// </summary>
        public string DocumentPath { get; private set; }

        /// <summary>
        ///     Gets the time a document was last seen
        /// </summary>
        public DateTime Timestamp { get; private set; }
    }
}