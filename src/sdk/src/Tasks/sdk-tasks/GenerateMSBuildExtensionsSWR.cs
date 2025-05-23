// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DotNet.Build.Tasks
{
    public class GenerateMSBuildExtensionsSWR : Task
    {
        [Required]
        public string MSBuildExtensionsLayoutDirectory { get; set; }

        [Required]
        public string OutputFile { get; set; }

        public override bool Execute()
        {
            StringBuilder sb = new StringBuilder(SWR_HEADER);

            AddFolder(sb,
                      @"MSBuildSdkResolver",
                      @"MSBuild\Current\Bin\SdkResolvers\Microsoft.DotNet.MSBuildSdkResolver",
                      ngenAssemblies: true);

            AddFolder(sb,
                      @"msbuildExtensions",
                      @"MSBuild");

            AddFolder(sb,
                      @"msbuildExtensions-ver",
                      @"MSBuild\Current");

            FileInfo outputFileInfo = new FileInfo(OutputFile);
            outputFileInfo.Directory.Create();
            File.WriteAllText(outputFileInfo.FullName, sb.ToString());

            return true;
        }

        private void AddFolder(StringBuilder sb, string relativeSourcePath, string swrInstallDir, bool ngenAssemblies = false)
        {
            string sourceFolder = Path.Combine(MSBuildExtensionsLayoutDirectory, relativeSourcePath);
            var files = Directory.GetFiles(sourceFolder)
                            .Where(f => !Path.GetExtension(f).Equals(".pdb", StringComparison.OrdinalIgnoreCase) && !Path.GetExtension(f).Equals(".swr", StringComparison.OrdinalIgnoreCase))
                            .ToList();
            if (files.Any(f => !Path.GetFileName(f).Equals("_._")))
            {
                sb.Append(@"folder ""InstallDir:\");
                sb.Append(swrInstallDir);
                sb.AppendLine(@"\""");

                foreach (var file in files)
                {
                    sb.Append(@"  file source=""$(PkgVS_Redist_Common_Net_Core_SDK_MSBuildExtensions)\");
                    sb.Append(Path.Combine(relativeSourcePath, Path.GetFileName(file)));
                    sb.Append('"');

                    if (ngenAssemblies && file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append(@" vs.file.ngenApplications=""[installDir]\Common7\IDE\vsn.exe""");
                    }

                    sb.AppendLine();
                }

                sb.AppendLine();
            }

            foreach (var subfolder in Directory.GetDirectories(sourceFolder))
            {
                string subfolderName = Path.GetFileName(subfolder);
                string newRelativeSourcePath = Path.Combine(relativeSourcePath, subfolderName);
                string newSwrInstallDir = Path.Combine(swrInstallDir, subfolderName);

                // Don't propagate ngenAssemblies to subdirectories.
                AddFolder(sb, newRelativeSourcePath, newSwrInstallDir);
            }
        }

        readonly string SWR_HEADER = @"use vs

package name=Microsoft.Net.Core.SDK.MSBuildExtensions
        version=$(ProductsBuildVersion)
        vs.package.internalRevision=$(PackageInternalRevision)

";
    }
}
