// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Microsoft.DotNet.Cli.Commands.Tool.Search;
using Microsoft.DotNet.Cli.NugetSearch.NugetSearchApiSerializable;
using Microsoft.DotNet.Cli.ToolPackage;

namespace Microsoft.DotNet.Cli.NugetSearch;

internal static class NugetSearchApiResultDeserializer
{
    public static IReadOnlyCollection<SearchResultPackage> Deserialize(string json)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new AuthorsConverter() },
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var deserialized = JsonSerializer.Deserialize<NugetSearchApiContainerSerializable>(json, options);
        var resultPackages = new List<SearchResultPackage>();
        foreach (var deserializedPackage in deserialized.Data)
        {
            var versions =
                deserializedPackage.Versions.Select(v => new SearchResultPackageVersion(v.Version, v.Downloads))
                    .ToArray();

            string[] authors = deserializedPackage?.Authors?.Authors ?? [];

            var searchResultPackage = new SearchResultPackage(new PackageId(deserializedPackage.Id),
                deserializedPackage.Version, deserializedPackage.Description, deserializedPackage.Summary,
                deserializedPackage.Tags, authors, deserializedPackage.TotalDownloads, deserializedPackage.Verified,
                versions);

            resultPackages.Add(searchResultPackage);
        }

        return resultPackages;
    }
}
