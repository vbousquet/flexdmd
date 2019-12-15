/* license

MFT.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Collections;
using System.Runtime.InteropServices;

using MediaFoundation.Interop;
using MediaFoundation.Misc;

namespace MediaFoundation.Transform
{
    public static class MFT
    {
        /// <summary>
        /// Enumerates Media Foundation transforms (MFTs) in the registry.
        /// </summary>
        /// <param name="transformCategory">Guid specifying the category of MFTs to enumerate.</param>
        /// <param name="inputType">Indicate the input media type that the results must support. If null is passed, the input type is not use to filter the results.</param>
        /// <param name="outputType">Indicate the output media type that the results must support. If null is passed, the output type is not use to filter the results.</param>
        /// <param name="resultCLSID">Receives an array of MFT CLSIDs that match the parameters.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Enum(Guid transformCategory, MFTRegisterTypeInfo inputType, MFTRegisterTypeInfo outputType, out Guid[] resultCLSID)
        {
            int resultCount;
            return MFExtern.MFTEnum(transformCategory, 0, inputType, outputType, null, out resultCLSID, out resultCount);
        }

        /// <summary>
        /// Enumerates Media Foundation transforms (MFTs) in the registry.
        /// </summary>
        /// <param name="transformCategory">Guid specifying the category of MFTs to enumerate.</param>
        /// <param name="inputType">Indicate the input media type that the results must support. If null is passed, the input type is not use to filter the results.</param>
        /// <param name="outputType">Indicate the output media type that the results must support. If null is passed, the output type is not use to filter the results.</param>
        /// <param name="attributes">Indicate attributes that the results must support.</param>
        /// <param name="resultCLSID">Receives an array of MFT CLSIDs that match the parameters.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>The parameter <paramref name="attributes"/> is ingnored on Windows 7 or higher and should be null.</remarks>
        public static HResult Enum(Guid transformCategory, MFTRegisterTypeInfo inputType, MFTRegisterTypeInfo outputType, IMFAttributes attributes, out Guid[] resultCLSID)
        {
            int resultCount;
            return MFExtern.MFTEnum(transformCategory, 0, inputType, outputType, attributes, out resultCLSID, out resultCount);
        }

        /// <summary>
        /// Enumerates Media Foundation transforms (MFTs) in the registry.
        /// </summary>
        /// <param name="transformCategory">Guid specifying the category of MFTs to enumerate.</param>
        /// <param name="flags">One or more members of the MFT_EnumFlag enumeration.</param>
        /// <param name="inputType">Indicate the input media type that the results must support. If null is passed, the input type is not use to filter the results.</param>
        /// <param name="outputType">Indicate the output media type that the results must support. If null is passed, the output type is not use to filter the results.</param>
        /// <param name="resultActivate">Receives an array of IMFActivate that match the parameters.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult EnumEx(Guid transformCategory, MFT_EnumFlag flags, MFTRegisterTypeInfo inputType, MFTRegisterTypeInfo outputType, out IMFActivate[] resultActivate)
        {
            int resultCount;
            return MFExtern.MFTEnumEx(transformCategory, flags, inputType, outputType, out resultActivate, out resultCount);
        }

        /// <summary>
        /// Gets information from the registry about a Media Foundation transform (MFT). 
        /// </summary>
        /// <param name="clsidMFT">The CLSID of the MFT to query.</param>
        /// <param name="name">Receives the name of the MFT.</param>
        /// <param name="inputTypes">Receives an array of input types supported by the MFT.</param>
        /// <param name="outputTypes">Receives an array of output types supported by the MFT.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetInfo(Guid clsidMFT, out string name, out MFTRegisterTypeInfo[] inputTypes, out MFTRegisterTypeInfo[] outputTypes)
        {
            ArrayList inTypes = new ArrayList();  
            ArrayList  outTypes = new ArrayList();
            MFInt inTypesCount = new MFInt();
            MFInt outTypesCount = new MFInt();

            HResult hr = MFExtern.MFTGetInfo(clsidMFT, out name, inTypes, inTypesCount, outTypes, outTypesCount, IntPtr.Zero);

            inputTypes = new MFTRegisterTypeInfo[inTypesCount];

            for (int i = 0; i < inTypesCount.ToInt32(); i++)
                inputTypes[i] = (MFTRegisterTypeInfo)inTypes[i];

            outputTypes = new MFTRegisterTypeInfo[outTypesCount];

            for (int i = 0; i < outTypesCount.ToInt32(); i++)
                outputTypes[i] = (MFTRegisterTypeInfo)outTypes[i];

            return hr;
        }

        /// <summary>
        /// Register the Media Foundation transform (MFT) into the registry.
        /// </summary>
        /// <param name="clsidMFT">The CLSID of the MFT.</param>
        /// <param name="transformCategory">Guid specifying the category of the MFT.</param>
        /// <param name="name">The MFT's firendly name.</param>
        /// <param name="flags">One or more members of the MFT_EnumFlag enumeration.</param>
        /// <param name="inputTypes">An array specifiying the MFT's input types.</param>
        /// <param name="outputTypes">An array specifiying the MFT's output types.</param>
        /// <param name="attributes">An attribute store containing additional registry information.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Register(Guid clsidMFT, Guid transformCategory, string name, MFT_EnumFlag flags, MFTRegisterTypeInfo[] inputTypes, MFTRegisterTypeInfo[] outputTypes, IMFAttributes attributes)
        {
            return MFExtern.MFTRegister(
                clsidMFT,
                transformCategory,
                name,
                flags,
                (inputTypes == null) ? 0 : inputTypes.Length,
                inputTypes,
                (outputTypes == null) ? 0 : outputTypes.Length,
                outputTypes,
                attributes
                );
        }

        /// <summary>
        /// Registers a Media Foundation Transform (MFT) in the caller's process.
        /// </summary>
        /// <param name="classFactory">A class factory object who have the responsibility to create the MFT.</param>
        /// <param name="transformCategory">Guid specifying the category of the MFT.</param>
        /// <param name="name">The MFT's firendly name.</param>
        /// <param name="flags">One or more members of the MFT_EnumFlag enumeration.</param>
        /// <param name="inputTypes">An array specifiying the MFT's input types.</param>
        /// <param name="outputTypes">An array specifiying the MFT's output types.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RegisterLocal(IClassFactory classFactory, Guid transformCategory, string name, MFT_EnumFlag flags, MFTRegisterTypeInfo[] inputTypes, MFTRegisterTypeInfo[] outputTypes)
        {
            return MFExtern.MFTRegisterLocal(
                classFactory,
                transformCategory,
                name,
                flags,
                (inputTypes == null) ? 0 : inputTypes.Length,
                inputTypes,
                (outputTypes == null) ? 0 : outputTypes.Length,
                outputTypes
                );
        }

        /// <summary>
        /// Registers a Media Foundation Transform (MFT) in the caller's process.
        /// </summary>
        /// <param name="clsidMFT">The CLSID of the MFT.</param>
        /// <param name="transformCategory">Guid specifying the category of the MFT.</param>
        /// <param name="name">The MFT's firendly name.</param>
        /// <param name="flags">One or more members of the MFT_EnumFlag enumeration.</param>
        /// <param name="inputTypes">An array specifiying the MFT's input types.</param>
        /// <param name="outputTypes">An array specifiying the MFT's output types.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RegisterLocalByCLSID(Guid clsidMFT, Guid transformCategory, string name, MFT_EnumFlag flags, MFTRegisterTypeInfo[] inputTypes, MFTRegisterTypeInfo[] outputTypes)
        {
            return MFExtern.MFTRegisterLocalByCLSID(
                clsidMFT,
                transformCategory,
                name,
                flags,
                (inputTypes == null) ? 0 : inputTypes.Length,
                inputTypes,
                (outputTypes == null) ? 0 : outputTypes.Length,
                outputTypes
                );
        }

        /// <summary>
        /// Unregister the Media Foundation transform (MFT) from the registry.
        /// </summary>
        /// <param name="clsidMFT">The CLSID of the MFT.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Unregister(Guid clsidMFT)
        {
            return MFExtern.MFTUnregister(clsidMFT);
        }

        /// <summary>
        /// Unregisters one or more Media Foundation transforms (MFTs) from the caller's process.
        /// </summary>
        /// <param name="classFactory">An instance of the <see cref="IClassFactory"/> interface of a class factory object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>If null is passed to <paramref name="classFactory"/> then all local MFT are unregistered.</remarks>
        public static HResult UnregisterLocal(IClassFactory classFactory)
        {
            return MFExtern.MFTUnregisterLocal(classFactory);
        }

        /// <summary>
        /// Unregisters a Media Foundation transform (MFT) from the caller's process.
        /// </summary>
        /// <param name="clsidMFT">The CLSID of the MFT.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult UnregisterLocalByCLSID(Guid clsidMFT)
        {
            return MFExtern.MFTUnregisterLocalByCLSID(clsidMFT);
        }
    }
}
