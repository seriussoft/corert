// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Internal.Runtime;

namespace System.Runtime
{
    public struct TypeManagerHandle
    {
        private IntPtr _handleValue;

        public TypeManagerHandle(IntPtr handleValue)
        {
            _handleValue = handleValue;
        }

        public IntPtr GetIntPtrUNSAFE()
        {
            return _handleValue;
        }

        public static bool operator ==(TypeManagerHandle left, TypeManagerHandle right)
        {
            return left._handleValue == right._handleValue;
        }

        public static bool operator !=(TypeManagerHandle left, TypeManagerHandle right)
        {
            return left._handleValue != right._handleValue;
        }

        public bool Equals(TypeManagerHandle other)
        {
            return _handleValue == other._handleValue;
        }
    }

    internal static class RuntimeImports
    {
        private const string RuntimeLibrary = "[MRT]";

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport(RuntimeLibrary, "RhpRegisterFrozenSegment")]
        internal static extern bool RhpRegisterFrozenSegment(IntPtr pSegmentStart, int length);

        [RuntimeImport(RuntimeLibrary, "RhpGetModuleSection")]
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private static extern IntPtr RhGetModuleSection(ref TypeManagerHandle module, ReadyToRunSectionType section, out int length);

        internal static IntPtr RhGetModuleSection(TypeManagerHandle module, ReadyToRunSectionType section, out int length)
        {
            return RhGetModuleSection(ref module, section, out length);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        [RuntimeImport(RuntimeLibrary, "RhpCreateTypeManager")]
        internal static extern unsafe TypeManagerHandle RhpCreateTypeManager(IntPtr osModule, IntPtr moduleHeader, IntPtr* pClasslibFunctions, int nClasslibFunctions);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        [RuntimeImport(RuntimeLibrary, "RhpRegisterOsModule")]
        internal static extern unsafe IntPtr RhpRegisterOsModule(IntPtr osModule);


        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport(RuntimeLibrary, "RhNewObject")]
        internal static extern object RhNewObject(EETypePtr pEEType);

        [MethodImplAttribute(MethodImplOptions.InternalCall)] 
        [RuntimeImport(RuntimeLibrary, "RhBulkMoveWithWriteBarrier")]
        internal static extern unsafe void RhBulkMoveWithWriteBarrier(ref byte dmem, ref byte smem, uint size);


        // Allocate handle.
        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport(RuntimeLibrary, "RhpHandleAlloc")]
        private static extern IntPtr RhpHandleAlloc(Object value, GCHandleType type);

        internal static IntPtr RhHandleAlloc(Object value, GCHandleType type)
        {
            IntPtr h = RhpHandleAlloc(value, type);
            if (h == IntPtr.Zero)
                throw new OutOfMemoryException();
            return h;
        }
    }
}
