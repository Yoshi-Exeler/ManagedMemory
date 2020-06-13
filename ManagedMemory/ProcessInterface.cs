using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;

namespace ManagedMemory
{
    public class ProcessInterface
    {
        protected Process managedProcess;
        protected IntPtr handle;

        public ProcessInterface(string name)
        {
            Process[] procs = Process.GetProcessesByName(name);
            if (procs.Length != 1) throw new Exception("Process is not unique or does not exist");
            managedProcess = procs[0];
            handle = API_OpenProcess(managedProcess.Id);
        }

        /*Searches the module for the specified pattern, then returns the address of the first byte of the pattern offset by the FinalOffset or null if the pattern was not found.
         * In most cases you should simply pass the name of the main modulue for the "module" argument.
         */
        public Address FindPattern(string module, byte[] pattern, string mask, int finalOffset)
        {
            ProcessModule mod = null;
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                if (pm.ModuleName == module) mod = pm;
            }
            if (mod == null) throw new ArgumentException("The Specified module " + module + " was not found");
            byte[] dump = new byte[mod.ModuleMemorySize];
            API_ReadProcessMemory(new Address(mod.BaseAddress), dump.Length);
            bool found = false;
            long resIndex = -1;
            for (int i = 0; i < dump.Length; i++)
            {
                for (int x = 0; x < pattern.Length; x++)
                {
                    if (i + x >= dump.Length || !(mask[x] == '?' || dump[i + x] == pattern[x])) break;
                    if (x == pattern.Length)
                    {
                        found = true;
                        resIndex = i;
                    }

                }
                if (found) break;
            }
            if (!found) return null;
            return new Address((long)mod.BaseAddress + resIndex + finalOffset);
        }

        public IntPtr GetHandle()
        {
            return handle;
        }

        public void InjectDll(string libraryPath)
        {
            IntPtr pathPointer = APIProxy.VirtualAllocEx(handle, IntPtr.Zero, (IntPtr)libraryPath.Length, APIProxy.AllocationType.Reserve | APIProxy.AllocationType.Commit, APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE);
            APIProxy.WriteProcessMemory(handle, pathPointer, Encoding.ASCII.GetBytes(libraryPath));
            IntPtr kernelDllPointer = APIProxy.GetModuleHandle("Kernel32.dll");
            IntPtr loadLibraryPointer = APIProxy.GetProcedureAddress(kernelDllPointer, "LoadLibraryA");
            IntPtr threadHandle = APIProxy.CreateRemoteThread(handle, IntPtr.Zero, 0, loadLibraryPointer, pathPointer, 0, IntPtr.Zero);
            APIProxy.CloseHandle(threadHandle);
        }

        public MemoryRegion AllocateMemory(int size, APIProxy.AllocationType allocationType = APIProxy.AllocationType.Reserve | APIProxy.AllocationType.Commit, APIProxy.MemoryProtection memoryProtection = APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE)
        {
            IntPtr allocation = APIProxy.VirtualAllocEx(handle, IntPtr.Zero, (IntPtr)size, allocationType, memoryProtection);
            if (allocation == null) throw new Exception("Allocation at an undefinded location for " + size + " Bytes  with  allocationType " + allocationType + " and memoryProtection " + memoryProtection + " has failed with the errorcode " + Marshal.GetLastWin32Error());
            MemoryRegion res = new MemoryRegion();
            res.start = new Address(allocation);
            res.lenght = size;
            return res;
        }

        public MemoryRegion AllocateMemoryAt(Address adr, int size, APIProxy.AllocationType allocationType = APIProxy.AllocationType.Reserve | APIProxy.AllocationType.Commit, APIProxy.MemoryProtection memoryProtection = APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE)
        {
            IntPtr allocation = APIProxy.VirtualAllocEx(handle, adr.GetAsPointer(), (IntPtr)size, allocationType, memoryProtection);
            if (allocation == null) throw new Exception("Allocation at " + adr + " location for " + size + " Bytes  with  allocationType " + allocationType + " and memoryProtection " + memoryProtection + " has failed with the errorcode " + Marshal.GetLastWin32Error());
            MemoryRegion res = new MemoryRegion();
            res.start = new Address(allocation);
            res.lenght = size;
            return res;
        }

        protected IntPtr API_OpenProcess(int pid)
        {
            IntPtr processHandle = APIProxy.OpenProcess(APIProxy.ProcessAccessFlags.All, pid);
            if (processHandle == null) throw new OpenProcessException("Obtaining a handle with Allaccess to the process with the pid " + pid + " has failed with errorcode " + Marshal.GetLastWin32Error());
            return processHandle;
        }

        protected byte[] API_ReadProcessMemory(Address adr, int size)
        {
            uint oldProtection = APIProxy.VirtualProtectEx(handle, adr.GetAsPointer(), size, (uint)APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE);
            byte[] buffer = APIProxy.ReadProcessMemory(handle, adr.GetAsPointer(), size);
            APIProxy.VirtualProtectEx(handle, adr.GetAsPointer(), size, oldProtection);
            return buffer;
        }

        protected void API_WriteProcessMemory(Address adr, byte[] val)
        {
            uint oldProtection = APIProxy.VirtualProtectEx(handle, adr.GetAsPointer(), val.Length, (uint)APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE);
            APIProxy.WriteProcessMemory(handle, adr.GetAsPointer(), val);
            APIProxy.VirtualProtectEx(handle, adr.GetAsPointer(), val.Length, oldProtection);
        }

        public Address GetModuleBase(string name)
        {
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                if (pm.ModuleName == name) return new Address(pm.BaseAddress);
            }
            return null;
        }

        public ArrayList GetLoadedModuleNames()
        {
            ArrayList res = new ArrayList();
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                res.Add(pm.ModuleName);
            }
            return res;
        }

        public ProcessModuleCollection GetLoadedModules()
        {
            return managedProcess.Modules;
        }

        public int GetPid()
        {
            return managedProcess.Id;
        }

        public void WriteInt32(Address adr, int val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteInt64(Address adr, long val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteFloat(Address adr, float val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteDouble(Address adr, double val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteByte(Address adr, byte val)
        {
            byte[] buf = { val };
            API_WriteProcessMemory(adr, buf);
        }

        public void WriteByteArray(Address adr, byte[] val)
        {
            API_WriteProcessMemory(adr, val);
        }

        public int ReadInt32(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, sizeof(int));
            return BitConverter.ToInt32(buffer, 0);
        }

        public long ReadInt64(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, sizeof(long));
            return BitConverter.ToInt64(buffer, 0);
        }

        public float ReadFloat(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, sizeof(float));
            return BitConverter.ToSingle(buffer, 0);
        }

        public double ReadDouble(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, sizeof(double));
            return BitConverter.ToDouble(buffer, 0);
        }

        public byte ReadByte(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, 1);
            return buffer[0];
        }

        public byte[] ReadByteArray(Address adr, int size)
        {
            byte[] buffer = API_ReadProcessMemory(adr, size);
            return buffer;
        }

        public void FreezeProcess()
        {
            foreach (ProcessThread pt in managedProcess.Threads)
            {
                IntPtr cHandle = IntPtr.Zero;
                cHandle = APIProxy.OpenThread(APIProxy.ThreadAccessFlags.THREAD_SUSPEND_RESUME, (uint)pt.Id);
                APIProxy.SuspendThread(cHandle);
            }
        }

        public void UnfreezeProcess()
        {
            foreach (ProcessThread pt in managedProcess.Threads)
            {
                IntPtr cHandle = IntPtr.Zero;
                cHandle = APIProxy.OpenThread(APIProxy.ThreadAccessFlags.THREAD_SUSPEND_RESUME, (uint)pt.Id);
                APIProxy.ResumeThread(cHandle);
            }
        }

        public void Terminate()
        {
            managedProcess.Kill();
        }
    }
}
