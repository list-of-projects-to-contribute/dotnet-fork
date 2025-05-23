﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Roslyn.LanguageServer.Protocol;

/// <summary>
/// Enum representing the default behavior used by the client for computing a rename range.
/// <para>
/// See the <see href="https://microsoft.github.io/language-server-protocol/specifications/specification-current/#prepareSupportDefaultBehavior">Language Server Protocol specification</see> for additional information.
/// </para>
/// </summary>
/// <remarks>Since LSP 3.16</remarks>
internal enum PrepareSupportDefaultBehavior
{
    /// <summary>
    /// The client's default behavior is to select the identifier according to the language's syntax rule.
    /// </summary>
    Identifier = 1,
}
