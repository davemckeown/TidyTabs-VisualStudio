// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Dave McKeown">
//   Apache 2.0 License
// </copyright>
// <summary>
//   Log class provides logging services for the extension
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DaveMcKeown.TidyTabs
{
    using System;
    using System.Text;

    using NLog;
    using NLog.Config;
    using NLog.Layouts;
    using NLog.Targets;

    /// <summary>
    ///     Log class provides logging services for the extension
    /// </summary>
    internal static class Log
    {
        /// <summary>
        ///     The NLog logger
        /// </summary>
        private static Logger logger;

        /// <summary>
        ///     Gets the Logger instance
        /// </summary>
        private static Logger Logger
        {
            get
            {
                return logger ?? (logger = Initialize());
            }
        }

        /// <summary>Logs an exception</summary>
        /// <param name="ex">The exception to log</param>
        public static void Exception(Exception ex)
        {
            Logger.Error(ex);
        }

        /// <summary>Logs a message</summary>
        /// <param name="msg">The message to log</param>
        public static void Message(string msg)
        {
            Logger.Info(msg);
        }

        /// <summary>Logs a formatted message</summary>
        /// <param name="format">String format</param>
        /// <param name="args">Argument values</param>
        public static void Message(string format, params object[] args)
        {
            Logger.Info(string.Format(format, args));
        }

        /// <summary>
        ///     Initializes the NLog configuration
        /// </summary>
        /// <returns>A NLog logger</returns>
        private static Logger Initialize()
        {
            LogManager.Configuration = new LoggingConfiguration();

            FileTarget target = new FileTarget();
            target.Name = "file";
            target.Layout = new SimpleLayout(@"${longdate} | ${level:uppercase=true} | ${message}");
            target.FileName = new SimpleLayout(@"${specialfolder:ApplicationData}\TidyTabs\Debug.log");
            target.ArchiveFileName = new SimpleLayout(@"${specialfolder:ApplicationData}\TidyTabs\Debug_Archive_{#}.log");
            target.ArchiveEvery = FileArchivePeriod.Day;
            target.ArchiveNumbering = ArchiveNumberingMode.Rolling;
            target.MaxArchiveFiles = 3;
            target.KeepFileOpen = false;
            target.CreateDirs = true;
            target.Encoding = Encoding.UTF8;

            LoggingRule loggingRule = new LoggingRule("*", LogLevel.Info, target);

            LogManager.Configuration.AddTarget("logfile", target);
            LogManager.Configuration.LoggingRules.Add(loggingRule);

            LogManager.ReconfigExistingLoggers();

            return LogManager.GetCurrentClassLogger();
        }
    }
}