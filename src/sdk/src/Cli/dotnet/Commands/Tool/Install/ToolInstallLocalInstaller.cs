// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CommandLine;
using Microsoft.DotNet.Cli.Extensions;
using Microsoft.DotNet.Cli.NuGetPackageDownloader;
using Microsoft.DotNet.Cli.ToolPackage;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.EnvironmentAbstractions;
using NuGet.Versioning;

namespace Microsoft.DotNet.Cli.Commands.Tool.Install;

internal class ToolInstallLocalInstaller
{
    private readonly ParseResult _parseResult;
    public string TargetFrameworkToInstall { get; private set; }

    private readonly IToolPackageDownloader _toolPackageDownloader;
    private readonly string _configFilePath;
    private readonly string[] _sources;
    private readonly VerbosityOptions _verbosity;
    private readonly RestoreActionConfig _restoreActionConfig;

    public ToolInstallLocalInstaller(
        ParseResult parseResult,
        IToolPackageDownloader toolPackageDownloader = null,
        string runtimeJsonPathForTests = null,
        RestoreActionConfig restoreActionConfig = null)
    {
        _parseResult = parseResult;
        _configFilePath = parseResult.GetValue(ToolInstallCommandParser.ConfigOption);
        _sources = parseResult.GetValue(ToolInstallCommandParser.AddSourceOption);
        _verbosity = parseResult.GetValue(ToolInstallCommandParser.VerbosityOption);

        (IToolPackageStore store,
            IToolPackageStoreQuery,
            IToolPackageDownloader downloader) toolPackageStoresAndDownloader
                = ToolPackageFactory.CreateToolPackageStoresAndDownloader(
                    additionalRestoreArguments: parseResult.OptionValuesToBeForwarded(ToolInstallCommandParser.GetCommand()), runtimeJsonPathForTests: runtimeJsonPathForTests);
        _toolPackageDownloader = toolPackageDownloader ?? toolPackageStoresAndDownloader.downloader;
        _restoreActionConfig = restoreActionConfig;

        TargetFrameworkToInstall = BundledTargetFramework.GetTargetFrameworkMoniker();
    }

    public IToolPackage Install(FilePath manifestFile, PackageId packageId)
    {
        if (!string.IsNullOrEmpty(_configFilePath) && !File.Exists(_configFilePath))
        {
            throw new GracefulException(
                string.Format(
                    CliCommandStrings.ToolInstallNuGetConfigurationFileDoesNotExist,
                    Path.GetFullPath(_configFilePath)));
        }

        VersionRange versionRange = _parseResult.GetVersionRange();

        FilePath? configFile = null;
        if (!string.IsNullOrEmpty(_configFilePath))
        {
            configFile = new FilePath(_configFilePath);
        }

        try
        {
            IToolPackage toolDownloadedPackage = _toolPackageDownloader.InstallPackage(
                    new PackageLocation(
                        nugetConfig: configFile,
                        additionalFeeds: _sources,
                        rootConfigDirectory: manifestFile.GetDirectoryPath().GetParentPath()),
                    packageId,
                    verbosity: _verbosity,
                    versionRange,
                    TargetFrameworkToInstall,
                    restoreActionConfig: _restoreActionConfig
                    );

            return toolDownloadedPackage;
        }
        catch (Exception ex) when (InstallToolCommandLowLevelErrorConverter.ShouldConvertToUserFacingError(ex))
        {
            throw new GracefulException(
                messages: InstallToolCommandLowLevelErrorConverter.GetUserFacingMessages(ex, packageId),
                verboseMessages: [ex.ToString()],
                isUserError: false);
        }
    }
}
