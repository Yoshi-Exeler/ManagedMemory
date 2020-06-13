﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Diagnostics;

namespace ManagedMemory
{
    public class WINAPI
    {
        [Flags]
        public enum ThreadAccessFlags : uint
        {
            THREAD_TERMINATE = 0x1,
            THREAD_SUSPEND_RESUME = 0x2,
            THREAD_GET_CONTEXT = 0x8,
            THREAD_SET_CONTEXT = 0x10,
            THREAD_SET_INFORMATION = 0x20,
            THREAD_SET_THREAD_TOKEN = 0x80,
            THREAD_IMPERSONATE = 0x100,
            THREAD_DIRECT_IMPERSONATION = 0x200,
            THREAD_SET_LIMITED_INFORMATION = 0x400,
        };

        [Flags]
        public enum MemoryProtection : uint
        {
            PAGE_NO_ACCESS = 0x1,
            PAGE_READ_ONLY = 0x2,
            PAGE_READ_WRITE = 0x4,
            PAGE_WRITE_COPY = 0x8,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READ_WRITE = 0x40,
            PAGE_EXECUTE_WRITE_COPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NO_CACHE = 0x200,
            PAGE_WRITE_COMBINE = 0x400
        };


        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 2035711u,
            Terminate = 1u,
            CreateThread = 2u,
            VirtualMemoryOperation = 8u,
            VirtualMemoryRead = 16u,
            VirtualMemoryWrite = 32u,
            DuplicateHandle = 64u,
            CreateProcess = 128u,
            SetQuota = 256u,
            SetInformation = 512u,
            QueryInformation = 1024u,
            QueryLimitedInformation = 4096u,
            Synchronize = 1048576u
        }

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr processHandle, IntPtr threadAttributesPointer, uint stackSize, IntPtr startAddress, IntPtr parameterPointer, uint creationFlags, IntPtr threadId);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr moduleHandle, string processName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr processHandle, IntPtr startAddress, IntPtr size, AllocationType allocationType, MemoryProtection protection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags accessFlags, bool inheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr targetAddress, [Out] byte[] output, int outputSize, ref long numberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr processHandle, IntPtr targetAddress, byte[] input, int inputSize, ref long numberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr processHandle, IntPtr regionStart, int regionSize, uint newProtection, out uint oldProtection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccessFlags desiredAccess, bool inheritHandle, uint threadID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SuspendThread(IntPtr threadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(IntPtr threadHandle);
    }
}
