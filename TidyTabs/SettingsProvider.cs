// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsProvider.cs" company="Dave McKeown">
//   Apache 2.0 License
// </copyright>
// <summary>
//   Provides consistent access to Tidy Tabs settings
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DaveMcKeown.TidyTabs
{
    using DaveMcKeown.TidyTabs.Properties;

    /// <summary>
    ///     Provides consistent access to Tidy Tabs settings
    /// </summary>
    internal class SettingsProvider
    {
        /// <summary>
        ///     The settings instance
        /// </summary>
        private static Settings settings;

        /// <summary>
        ///     Gets the current settings instance
        /// </summary>
        internal static Settings Instance
        {
            get
            {
                return settings ?? (settings = Settings.Default);
            }
        }
    }
}