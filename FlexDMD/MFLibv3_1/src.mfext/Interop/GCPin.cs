/* license

GCPin.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace MediaFoundation.Interop
{
    /// <summary>
    /// A wrapper around <see cref="GCHandle"/> that implement the <see cref="IDisposable"/> pattern.
    /// </summary>
    public class GCPin : IDisposable
    {
        private GCHandle m_handle;

        protected GCPin() { }

        /// <summary>
        /// Pin an object so that it can't be collected or moved by the garbage-collector.
        /// </summary>
        /// <param name="pinned">The object to pin.</param>
        public GCPin(object pinned)
        {
            if (pinned == null)
                throw new ArgumentNullException("pinned");

            m_handle = GCHandle.Alloc(pinned, GCHandleType.Pinned);
        }

        /// <summary>
        /// Gets the address of the pinned object.
        /// </summary>
        public IntPtr PinnedAddress
        {
            get { return this.GetPinnedAddress(); }
        }

        /// <summary>
        /// Release the pin on the object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_handle.IsAllocated)
                    m_handle.Free();
            }
        }

        protected virtual IntPtr GetPinnedAddress()
        {
            return m_handle.AddrOfPinnedObject();
        }
    }

    /// <summary>
    /// An implementation of <see cref="GCPin"/> for value types.
    /// </summary>
    /// <typeparam name="T">A value type.</typeparam>
    /// <remarks>
    /// <p>The first use of this object (for a given type) can be very slow because of the creation of a dynamic methods. The next usages are a lot faster (about 3 times slower than using unsafe pointers).</p>
    /// <p>Warning: This object don't pin the value type at all so use it for the shortest time as possible.</p>
    /// </remarks>
    public class GCPin<T> : GCPin where T : struct
    {
        private IntPtr m_valueTypePtr;

        /// <summary>
        /// Get the address of a value type
        /// </summary>
        /// <param name="value">The value type to get the address of.</param>
        public GCPin(ref T value) : base()
        {
            unsafe
            {
                m_valueTypePtr = new IntPtr(GCPinMethodCache.GetPointerMethod<T>()(ref value));
            }
        }

        /// <summary>
        /// Gets the address of the value type.
        /// </summary>
        protected override IntPtr GetPinnedAddress()
        {
            return m_valueTypePtr;
        }

        protected override void Dispose(bool disposing)
        {
            m_valueTypePtr = IntPtr.Zero;
        }
    }

    internal unsafe delegate void* GetPointerMethod<T>(ref T refType) where T : struct;

    internal static class GCPinMethodCache
    {
        static Dictionary<Type, Delegate> s_methodCache;

        static GCPinMethodCache()
        {
            s_methodCache = new Dictionary<Type, Delegate>();
            
            // Generate dynamic methods for common types
            s_methodCache.Add(typeof(int), GenerateGetPointerMethod<int>());
            s_methodCache.Add(typeof(long), GenerateGetPointerMethod<long>());
            s_methodCache.Add(typeof(float), GenerateGetPointerMethod<float>());
            s_methodCache.Add(typeof(double), GenerateGetPointerMethod<double>());
        }

        public static GetPointerMethod<T> GetPointerMethod<T>() where T : struct
        {
            Type typeOfT = typeof(T);

            if (!s_methodCache.ContainsKey(typeOfT))
                s_methodCache.Add(typeOfT, GenerateGetPointerMethod<T>());

            return (GetPointerMethod<T>)s_methodCache[typeOfT];
        }

        private static GetPointerMethod<T> GenerateGetPointerMethod<T>() where T : struct
        {
            // Generate a method similar to :
            //
            // public static unsafe void* GetPointerNameOfT(ref T value)
            // {
            //      return (void*)&value;
            // }

            Type typeOfT = typeof(T);

            DynamicMethod method = new DynamicMethod(string.Format("GetPointer{0}", typeOfT.Name), typeof(void*), new Type[] { typeOfT.MakeByRefType() }, typeof(GCPin<T>));
            ILGenerator il = method.GetILGenerator();

            // A local variable to temporary store the address location
            il.DeclareLocal(typeOfT.MakeByRefType());

            // Load first parameter & store address location
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc_0);

            // Load address location & convert as native int (IntPtr)
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Conv_I);

            // Return
            il.Emit(OpCodes.Ret);

            return (GetPointerMethod<T>)method.CreateDelegate(typeof(GetPointerMethod<T>));
        }
    }
}
