﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.DotNet.Cli.Utils;

namespace Microsoft.DotNet.Cli.Commands.Fsi;

public class FsiForwardingApp(string[] arguments) : ForwardingApp(GetFsiAppPath(), processArguments(arguments))
{
    private const string FsiDllName = @"FSharp/fsi.dll";
    private const string FsiExeName = @"FSharp/fsi.exe";

    static string[] processArguments(string[] args)
    {
        var lang = UILanguageOverride.GetOverriddenUILanguage();
        if (lang == null)
        {
            return args;
        }
        else
        {
            return [.. args, $"--preferreduilang:{lang.Name}"];
        }
    }

    private static bool exists(string path)
    {
        try
        {
            return File.Exists(path);
        }
        catch
        {
            return false;
        }

    }

    /*
     * FSharp switched from compiling fsi.exe to fsi.dll which will allow us to ship an AppHost version of fsi.exe
     * The signal that fsi.exe is an apphost fs.exe is the presence of fsi.dll
     *
     * So here we look for fsi.dll, if it's found then we will return the path to it, otherwise we return fsi.exe
     * the reason for using this bridging mechanism is to simplify the coordination between F#/VS and the dotnet sdk
    */
    private static string GetFsiAppPath()
    {
        var dllPath = Path.Combine(AppContext.BaseDirectory, FsiDllName);
        if (exists(dllPath))
        {
            return dllPath;
        }
        else
        {
            return Path.Combine(AppContext.BaseDirectory, FsiExeName);
        }
    }
}
