// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TidyTabsOptionPage.cs" company="Dave McKeown">
//   Apache 2.0 License
// </copyright>
// <summary>
//   TidyTabsOptionPage provides the ability to manage settings for TidyTabs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DaveMcKeown.TidyTabs
{
    using DaveMcKeown.TidyTabs.Attributes;
    using DaveMcKeown.TidyTabs.Properties;

    using Microsoft.VisualStudio.Shell;

    /// <summary>
    ///     TidyTabsOptionPage provides the ability to manage settings for TidyTabs
    /// </summary>
    public class TidyTabsOptionPage : DialogPage
    {
        /// <summary>
        ///     Tidy Tabs settings
        /// </summary>
        private Settings settings;

        /// <summary>
        ///     Gets or sets a value indicating whether tabs should be purged when a file is saved
        /// </summary>
        [LocalizedCategory("OptionPage_Behavior")]
        [LocalizedDisplayName("PurgeOnSave_DisplayName")]
        [LocalizedDescription("PurgeOnSave_Description")]
        public bool PurgeTabsOnSave
        {
            get
            {
                return Settings.PurgeStaleTabsOnSave;
            }

            set
            {
                Settings.PurgeStaleTabsOnSave = value;
                Settings.Save();
            }
        }

        /// <summary>
        ///     Gets or sets the timeout for tabs before they are marked as stale
        /// </summary>
        [LocalizedCategory("OptionPage_Settings")]
        [LocalizedDisplayName("TabTimeoutMinutes_DisplayName")]
        [LocalizedDescription("TabTimeoutMinutes_Description")]
        public int TabTimeoutMinutes
        {
            get
            {
                return Settings.TabTimeoutMinutes;
            }

            set
            {
                Settings.TabTimeoutMinutes = value;
                Settings.Save();
            }
        }

        /// <summary>
        ///     Gets or sets the threshold for number of open tabs at which point to start closing tabs
        /// </summary>
        [LocalizedCategory("OptionPage_Settings")]
        [LocalizedDisplayName("TabCloseThreshold_DisplayName")]
        [LocalizedDescription("TabCloseThreshold_Description")]
        public int TabCloseThreshold
        {
            get
            {
                return Settings.TabCloseThreshold;
            }

            set
            {
                Settings.TabCloseThreshold = value;
                Settings.Save();
            }
        }

        /// <summary>
        ///     Gets or sets the maximum number of saved open document tabs
        /// </summary>
        [LocalizedCategory("OptionPage_Settings")]
        [LocalizedDisplayName("MaxOpenTabs_DisplayName")]
        [LocalizedDescription("MaxOpenTabs_Description")]
        public int MaxOpenTabs
        {
            get
            {
                return Settings.MaxOpenTabs;
            }

            set
            {
                Settings.MaxOpenTabs = value;
                Settings.Save();
            }
        }

        /// <summary>
        ///     Gets the Settings
        /// </summary>
        private Settings Settings
        {
            get
            {
                return settings ?? (settings = SettingsProvider.Instance);
            }
        }
    }
}