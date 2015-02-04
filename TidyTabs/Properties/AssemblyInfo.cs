// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Dave McKeown">
//   Apache 2.0 License
// </copyright>
// <summary>
//   AssemblyInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using DaveMcKeown.TidyTabs;

[assembly: AssemblyTitle("Tidy Tabs")]
[assembly: AssemblyDescription("Tidy Tabs Visual Studio extension")]
#if DEBUG

[assembly: AssemblyConfiguration("Debug")]
#else

[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("Dave McKeown")]
[assembly: AssemblyProduct("Tidy Tabs")]
[assembly: AssemblyCopyright("Apache 2.0 License")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyVersion(TidyTabsPackage.Version)]
[assembly: AssemblyFileVersion(TidyTabsPackage.Version)]