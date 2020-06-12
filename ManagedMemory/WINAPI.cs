using System;
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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, ref long lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, ref long lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, IntPtr nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle,
        uint dwThreadId);

        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtr hThread);
    }
}
