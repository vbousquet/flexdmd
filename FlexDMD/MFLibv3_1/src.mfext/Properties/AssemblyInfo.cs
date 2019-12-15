/* license

AssemblyInfo.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MediaFoundation .Net Extension Library")]
[assembly: AssemblyDescription("An extension assembly the MediaFoundation .Net Library. See http://mfnet.sourceforge.net/")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyVersion("1.0.0.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]
#if DEBUG
[assembly: AssemblyProduct("Debug Version")]
#else
[assembly : AssemblyProduct("Release Version")]
#endif
[assembly: AssemblyCopyright("GNU Lesser General Public License v2.1 or BSD")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("89684fd4-831e-4c9d-8bc9-7ea040dcfa95")]
