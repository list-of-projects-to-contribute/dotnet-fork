// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Internal;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(BuildInfo.PresentationFramework)]

// Add references for the themes.  This will only add support for generic.xaml
// This will have to be edited to add support for regular theme files (if required).
[assembly: System.Windows.ThemeInfoAttribute(System.Windows.ResourceDictionaryLocation.SourceAssembly, System.Windows.ResourceDictionaryLocation.SourceAssembly)]
