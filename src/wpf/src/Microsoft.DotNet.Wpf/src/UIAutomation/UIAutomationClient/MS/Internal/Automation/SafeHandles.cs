﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// 
//
// Description: Various SafeHandles used by UIA
//

using System;
using System.Runtime.InteropServices;

namespace MS.Internal.Automation
{
    internal sealed class SafeNodeHandle : SafeHandle
    {
        // Called by P/Invoke when returning SafeHandles
        // (Also used by UiaCoreApi to create invalid handles.)
        internal SafeNodeHandle()
            : base(IntPtr.Zero, true)
        {
        }

        // No need to provide a finalizer - SafeHandle's critical finalizer will
        // call ReleaseHandle for you.
        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            return UiaCoreApi.UiaNodeRelease(handle);
        }
    }


    // Internal Class that wraps the IntPtr to the Pattern
    internal sealed class SafePatternHandle : SafeHandle
    {
        // Called by P/Invoke when returning SafeHandles
        // (Also used by UiaCoreApi to create invalid handles.)
        internal SafePatternHandle()
            : base(IntPtr.Zero, true)
        {
        }
        // No need to provide a finalizer - SafeHandle's critical finalizer will
        // call ReleaseHandle for you.
        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            return UiaCoreApi.UiaPatternRelease(handle);
        }
    }


    // Internal Class that wraps the IntPtr to the Event
    internal sealed class SafeEventHandle : SafeHandle
    {
        internal SafeEventHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            UiaCoreApi.UiaRemoveEvent(handle);
            return true;
        }
    }
}
