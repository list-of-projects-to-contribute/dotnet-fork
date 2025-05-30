﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.FindUsages;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.MetadataAsSource;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.TestHooks;
using Microsoft.CodeAnalysis.Text;
using Roslyn.LanguageServer.Protocol;
using LSP = Roslyn.LanguageServer.Protocol;

namespace Microsoft.CodeAnalysis.LanguageServer.Handler;

[ExportCSharpVisualBasicStatelessLspService(typeof(FindAllReferencesHandler)), Shared]
[Method(LSP.Methods.TextDocumentReferencesName)]
internal sealed class FindAllReferencesHandler : ILspServiceDocumentRequestHandler<VSInternalReferenceParams, LSP.SumType<VSInternalReferenceItem, LSP.Location>[]?>
{
    private readonly IMetadataAsSourceFileService _metadataAsSourceFileService;
    private readonly IAsynchronousOperationListener _asyncListener;
    private readonly IGlobalOptionService _globalOptions;

    [ImportingConstructor]
    [Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
    public FindAllReferencesHandler(
        IMetadataAsSourceFileService metadataAsSourceFileService,
        IAsynchronousOperationListenerProvider asynchronousOperationListenerProvider,
        IGlobalOptionService globalOptions)
    {
        _metadataAsSourceFileService = metadataAsSourceFileService;
        _asyncListener = asynchronousOperationListenerProvider.GetListener(FeatureAttribute.LanguageServer);
        _globalOptions = globalOptions;
    }

    public bool MutatesSolutionState => false;
    public bool RequiresLSPSolution => true;

    public TextDocumentIdentifier GetTextDocumentIdentifier(VSInternalReferenceParams request) => request.TextDocument;

    public async Task<SumType<VSInternalReferenceItem, LSP.Location>[]?> HandleRequestAsync(
        VSInternalReferenceParams referenceParams,
        RequestContext context,
        CancellationToken cancellationToken)
    {
        var document = context.Document;
        var workspace = context.Workspace;
        Contract.ThrowIfNull(document);
        Contract.ThrowIfNull(workspace);

        var linePosition = ProtocolConversions.PositionToLinePosition(referenceParams.Position);
        var clientCapabilities = context.GetRequiredClientCapabilities();

        using var progress = BufferedProgress.Create(referenceParams.PartialResultToken);

        await FindReferencesAsync(progress, workspace, document, linePosition, clientCapabilities.HasVisualStudioLspCapability(), _globalOptions, _metadataAsSourceFileService, _asyncListener, cancellationToken).ConfigureAwait(false);

        return progress.GetFlattenedValues();
    }

    internal static async Task FindReferencesAsync(
        IProgress<SumType<VSInternalReferenceItem, LSP.Location>[]> progress,
        Workspace workspace,
        Document document,
        LinePosition linePosition,
        bool supportsVSExtensions,
        IGlobalOptionService globalOptions,
        IMetadataAsSourceFileService metadataAsSourceFileService,
        IAsynchronousOperationListener asyncListener,
        CancellationToken cancellationToken)
    {
        var findUsagesService = document.GetRequiredLanguageService<IFindUsagesLSPService>();
        var position = await document.GetPositionFromLinePositionAsync(linePosition, cancellationToken).ConfigureAwait(false);

        var findUsagesContext = new FindUsagesLSPContext(
            progress, workspace, document, position, metadataAsSourceFileService, asyncListener, globalOptions, supportsVSExtensions, cancellationToken);

        // Finds the references for the symbol at the specific position in the document, reporting them via streaming to the LSP client.
        var classificationOptions = globalOptions.GetClassificationOptionsProvider();
        await findUsagesService.FindReferencesAsync(findUsagesContext, document, position, classificationOptions, cancellationToken).ConfigureAwait(false);
        await findUsagesContext.OnCompletedAsync(cancellationToken).ConfigureAwait(false);
    }
}
