// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizedCategory.cs" company="Dave McKeown">
//   Apache 2.0 License
// </copyright>
// <summary>
//   Provides access to localized description attribute
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DaveMcKeown.TidyTabs.Attributes
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using DaveMcKeown.TidyTabs.Properties;

    /// <summary>
    ///     Provides access to localized description attribute
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property
        | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter)]
    public class LocalizedCategory : CategoryAttribute
    {
        /// <summary>Initializes a new instance of the LocalizedCategory class</summary>
        /// <param name="key">The resource manager key</param>
        public LocalizedCategory(string key)
            : base(Resources.ResourceManager.GetString(key, CultureInfo.CurrentCulture))
        {
        }
    }
}