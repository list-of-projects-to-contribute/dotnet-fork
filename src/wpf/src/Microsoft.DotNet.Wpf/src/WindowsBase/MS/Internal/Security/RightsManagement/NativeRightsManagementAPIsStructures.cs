﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/// Description:
//  These are the Structure declarations for interop services required to call into unmanaged 
//  Promethium Rights Management SDK APIs 

#define PRESENTATION_HOST_DLL
// for "PresentationHostDLL.dll"

using System.Runtime.InteropServices;

namespace MS.Internal.Security.RightsManagement
{
    [StructLayout(LayoutKind.Sequential)]
    internal class ActivationServerInfo
    {
        public uint    Version;
        [MarshalAs( UnmanagedType.LPWStr )]internal string  PubKey = "";
        [MarshalAs( UnmanagedType.LPWStr )]internal string  Url  = "";
    }

    // Declare a class to represent unmanaged SYSTEMTIME structure expected by DRM SDK 
    [ StructLayout( LayoutKind.Sequential )]
    internal class SystemTime 
    {
        internal SystemTime (DateTime dateTime)
        {
            Year = (ushort)dateTime.Year; 
            Month =  (ushort)dateTime.Month;  
            DayOfWeek =  (ushort)dateTime.DayOfWeek; 
            Day =  (ushort)dateTime.Day; 
            Hour =  (ushort)dateTime.Hour; 
            Minute =  (ushort)dateTime.Minute; 
            Second =  (ushort)dateTime.Second; 
            Milliseconds =  (ushort)dateTime.Millisecond; 
        }

        internal static uint Size
        {
            get
            {
                return 8 * sizeof(short);
            }
        }

        // construct it from memory buffer 
        internal SystemTime(byte[]  dataBuffer) 
        {
            Year = BitConverter.ToUInt16(dataBuffer,0);  
            Month = BitConverter.ToUInt16(dataBuffer,2);  
            DayOfWeek = BitConverter.ToUInt16(dataBuffer,4);  
            Day = BitConverter.ToUInt16(dataBuffer,6);  
            Hour = BitConverter.ToUInt16(dataBuffer,8);  
            Minute = BitConverter.ToUInt16(dataBuffer,10);  
            Second = BitConverter.ToUInt16(dataBuffer,12);  
            Milliseconds = BitConverter.ToUInt16(dataBuffer,14);  
        }
       
        internal DateTime GetDateTime (DateTime defaultValue)
        {
            // It seems that unmanaged APIs use the all 0s values to indicate 
            // that Date Time isn't present 
            if ((Year == 0) &&
                (Month == 0) &&
                (Day == 0) &&
                (Hour == 0) &&
                (Minute == 0) &&
                (Second == 0) &&
                (Milliseconds == 0))
            {
                return defaultValue;
            }
            else
            {
                return new DateTime(Year, Month, Day,
                        Hour, Minute, Second, Milliseconds);
            }
        }
       
       private ushort Year =0; 
       private ushort Month =0; 
       private ushort DayOfWeek =0; 
       private ushort Day =0; 
       private ushort Hour =0; 
       private ushort Minute =0; 
       private ushort Second =0; 
       private ushort Milliseconds =0; 
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class BoundLicenseParams
    {
        internal uint     uVersion = 0;
        internal uint     hEnablingPrincipal = 0;
        internal uint     hSecureStore = 0;
        [MarshalAs( UnmanagedType.LPWStr )]public string  wszRightsRequested = null;
        [MarshalAs( UnmanagedType.LPWStr )]public string  wszRightsGroup = null;

        //Actual members of DRMID
        internal uint     DRMIDuVersion = 0;
        [MarshalAs( UnmanagedType.LPWStr )]public string  DRMIDIdType = null;
        [MarshalAs( UnmanagedType.LPWStr )]public string  DRMIDId = null;
    
        internal uint     cAuthenticatorCount = 0;//reserved.should be 0.

        internal IntPtr rghAuthenticators = IntPtr.Zero;
        
        [MarshalAs( UnmanagedType.LPWStr )]public string  wszDefaultEnablingPrincipalCredentials = null;
        internal uint     dwFlags = 0;
    }
}
