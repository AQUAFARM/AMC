using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using Schedulr;

[assembly: AssemblyVersion("3.2.*")] // This can be updated anytime as long as .NET user settings are not used (otherwise the settings directory depends on this version).
[assembly: AssemblyInformationalVersion("2.0.0.0")] // Keep this constant as long as possible to avoid the user's settings getting lost (it is used for the LocalUserAppDataPath where the configuration file is stored).

[assembly: AssemblyProduct(Constants.ApplicationName)]
[assembly: AssemblyTitle(Constants.ApplicationName)]
[assembly: AssemblyDescription("Automatically uploads pictures and videos to Flickr based on a schedule.")]
[assembly: AssemblyCompany("Jelle Druyts")]
[assembly: AssemblyCopyright("Copyright © Jelle Druyts 2011")]
[assembly: Guid("73de3688-fda5-453e-b320-c981dd87346e")]

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]