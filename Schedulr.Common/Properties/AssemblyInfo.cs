using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using Schedulr;

[assembly: AssemblyVersion("2.0.1.0")] // Keep this constant as long as there are no changes to the contracts.

[assembly: AssemblyProduct(Constants.ApplicationName)]
[assembly: AssemblyTitle(Constants.ApplicationName + " Common Library")]
[assembly: AssemblyDescription("Contains common types used both by the application and for extensibility.")]
[assembly: AssemblyCompany("Jelle Druyts")]
[assembly: AssemblyCopyright("Copyright © Jelle Druyts 2011")]
[assembly: Guid("3dbf2379-1f49-4d83-bdab-ea1c29ceaaf3")]

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en-US")]