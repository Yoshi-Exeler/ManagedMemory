using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ManagedMemory
{
    public static class APIProxy
    {
        public static void VirtualFreeEx(Handle processHandle, Address targetAddress, int size, APIProxy.FreeType freeType)
        {
            if (WINAPI.VirtualFreeEx(processHandle.GetHandleAsPointer(), targetAddress.GetAsPointer(), size, freeType) == false) throw new VirtualFreeException("Freeing " + size + " Byes at " + targetAddress + " failed with errorcode " + Marshal.GetLastWin32Error());
        }

        public static Handle GetModuleHandle(string moduleName)
        {
            IntPtr res = WINAPI.GetModuleHandle(moduleName);
            if (res == null) throw new GetModuleHandleException("Obtaining a handle for the module " + moduleName + " has failed with errorcode" + Marshal.GetLastWin32Error());
            return new Handle(res);
        }

        public static void CloseHandle(Handle handle)
        {
            if (WINAPI.CloseHandle(handle.GetHandleAsPointer()) == false) throw new CloseHandleException("Closing the handle " + handle + " has failed with errorcode+" + Marshal.GetLastWin32Error());
        }

        public static Handle CreateRemoteThread(Handle processHandle, Address threadAttributesPointer, uint stackSize, Address startAddress, Address parameterPointer, uint creationFlags, int threadId)
        {
            IntPtr res = WINAPI.CreateRemoteThread(processHandle.GetHandleAsPointer(), threadAttributesPointer.GetAsPointer(), stackSize, startAddress.GetAsPointer(), parameterPointer.GetAsPointer(), creationFlags, (IntPtr)threadId);
            if (res == null) throw new CreateRemoteThreadException("Creating a remote thread in the process " + processHandle + " with the threadattributePointer " + threadAttributesPointer + " stacksize " + stackSize + " startaddress " + startAddress + " parameterPointer " + parameterPointer + " creationFlags " + creationFlags + " and threadId " + threadId + " has failed with errorcode " + Marshal.GetLastWin32Error());
            return new Handle(res);
        }

        public static Address GetProcedureAddress(Handle moduleHandle, string procedureName)
        {
            IntPtr res = WINAPI.GetProcAddress(moduleHandle.GetHandleAsPointer(), procedureName);
            if (res == null) throw new GetProcedureAddressException("Getting the procedure address for the procedure named " + procedureName + " in the module with the handle " + moduleHandle + " has failed with errorcode " + Marshal.GetLastWin32Error());
            return new Address(res);
        }

        public static MemoryRegion VirtualAllocEx(Handle processHandle, Address startAddress, int size, AllocationType allocationType, MemoryProtection protection)
        {
            IntPtr res = WINAPI.VirtualAllocEx(processHandle.GetHandleAsPointer(), startAddress.GetAsPointer(), (IntPtr)size, allocationType, protection);
            if (res == null) throw new VirtualAllocationException("Allocating " + size + " Bytes at " + startAddress + " with the allocationType " + allocationType + " and the protection " + protection + " has failed with errorcode " + Marshal.GetLastWin32Error());
            MemoryRegion region = new MemoryRegion();
            region.start = new Address(res);
            region.lenght = (int)size;
            return region;
        }

        public static Handle OpenProcess(ProcessAccessFlags accessFlags, int processId)
        {
            IntPtr res = WINAPI.OpenProcess(accessFlags, false, processId);
            if (res == null) throw new OpenProcessException("Getting a handle with " + accessFlags + " access to the process with the id " + processId + " has failed with errorcode " + Marshal.GetLastWin32Error());
            return new Handle(res);
        }

        public static byte[] ReadProcessMemory(Handle processHandle, Address targetAddress, int outputSize)
        {
            byte[] buffer = new byte[outputSize];
            long bytesRead = 0;
            if (WINAPI.ReadProcessMemory(processHandle.GetHandleAsPointer(), targetAddress.GetAsPointer(), buffer, outputSize, ref bytesRead) == false) throw new ReadProcessMemoryException("Reading " + outputSize + " Bytes from " + targetAddress + " has failed with errorcode " + Marshal.GetLastWin32Error());
            return buffer;
        }

        public static void WriteProcessMemory(Handle processHandle, Address targetAddress, byte[] input)
        {
            long bytesWritten = 0;
            if (WINAPI.WriteProcessMemory(processHandle.GetHandleAsPointer(), targetAddress.GetAsPointer(), input, input.Length, ref bytesWritten) == false) throw new WriteProcessMemoryException("Writing " + input.Length + " Bytes to " + targetAddress + " has failed with errorcode " + Marshal.GetLastWin32Error());
        }

        public static uint VirtualProtectEx(Handle processHandle, Address regionStart, int regionSize, uint newProtection)
        {
            uint oldProtection = 0;
            if (WINAPI.VirtualProtectEx(processHandle.GetHandleAsPointer(), regionStart.GetAsPointer(), regionSize, newProtection, out oldProtection) == false) throw new VirtualProtectionException("Protecting " + regionStart + " to " + regionStart + " + " + regionSize + " with the protection " + newProtection + " has failed with errorcode" + Marshal.GetLastWin32Error());
            return oldProtection;
        }

        public static Handle OpenThread(ThreadAccessFlags desiredAccess, uint threadID)
        {
            IntPtr res = WINAPI.OpenThread(desiredAccess, false, threadID);
            if (res == null) throw new OpenThreadException("Getting a handle with " + desiredAccess + " access for the thread with the id " + threadID + " has failed with errorcode" + Marshal.GetLastWin32Error());
            return new Handle(res);
        }

        public static void SuspendThread(Handle threadHandle)
        {
            WINAPI.SuspendThread(threadHandle.GetHandleAsPointer());
        }

        public static void ResumeThread(Handle threadHandle)
        {
            WINAPI.ResumeThread(threadHandle.GetHandleAsPointer());
        }

        public static uint GetProcessIDFromThread(Handle threadHandle)
        {
            uint res = WINAPI.GetProcessIdOfThread(threadHandle.GetHandleAsPointer());
            if (res == 0) throw new GetProcessIDFromThreadException("Getting the process id of the process belonging to the thread with the handle " + threadHandle + " has failed with errorcode " + Marshal.GetLastWin32Error());
            return res;
        }

        [Flags]
        public enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000,
        }

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
    }
}
