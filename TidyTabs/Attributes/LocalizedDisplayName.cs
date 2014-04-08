// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizedDisplayName.cs" company="Dave McKeown">
//   Apache 2.0 License
// </copyright>
// <summary>
//   Provides access to localized display name attribute
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DaveMcKeown.TidyTabs.Attributes
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using DaveMcKeown.TidyTabs.Properties;

    /// <summary>
    ///     Provides access to localized display name attribute
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property
        | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter)]
    public class LocalizedDisplayName : DisplayNameAttribute
    {
        /// <summary>Initializes a new instance of the LocalizedDisplayName class</summary>
        /// <param name="key">Resource manager key</param>
        public LocalizedDisplayName(string key)
            : base(Resources.ResourceManager.GetString(key, CultureInfo.CurrentCulture))
        {
        }
    }
}