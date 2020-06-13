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
    internal class WINAPI
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetProcessIdOfThread(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr processHandle, IntPtr threadAttributesPointer, uint stackSize, IntPtr startAddress, IntPtr parameterPointer, uint creationFlags, IntPtr threadId);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr moduleHandle, string procedureName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr processHandle, IntPtr startAddress, IntPtr size, APIProxy.AllocationType allocationType, APIProxy.MemoryProtection protection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(APIProxy.ProcessAccessFlags accessFlags, bool inheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr targetAddress, [Out] byte[] output, int outputSize, ref long numberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr processHandle, IntPtr targetAddress, byte[] input, int inputSize, ref long numberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr processHandle, IntPtr regionStart, int regionSize, uint newProtection, out uint oldProtection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(APIProxy.ThreadAccessFlags desiredAccess, bool inheritHandle, uint threadID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SuspendThread(IntPtr threadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(IntPtr threadHandle);
    }
}
