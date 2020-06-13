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
        private Process managedProcess;
        private IntPtr handle;

        public ProcessInterface(string name)
        {
            Process[] procs = Process.GetProcessesByName(name);
            if (procs.Length != 1) throw new Exception("Process is not unique or does not exist");
            managedProcess = procs[0];
            handle = api_OpenProcess(managedProcess.Id);
        }

        /*Searches the module for the specified pattern, then returns the address of the first byte of the pattern offset by the FinalOffset or null if the pattern was not found.
         * In most cases you should simply pass the name of the main modulue for the "module" argument.
         */
        public Address findPattern(string module, byte[] pattern, string mask, int finalOffset)
        {
            ProcessModule mod = null;
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                if (pm.ModuleName == module) mod = pm;
            }
            if (mod == null) throw new ArgumentException("The Specified module " + module + " was not found");
            byte[] dump = new byte[mod.ModuleMemorySize];
            api_ReadProcessMemory(new Address(mod.BaseAddress), dump.Length);
            bool found = false;
            long resIndex = -1;
            for (int i = 0; i < dump.Length; i++)
            {
                for (int x = 0; x < pattern.Length; x++)
                {
                    if (i + x > dump.Length || !(mask[x] == '?' || dump[i + x] == pattern[x])) break;
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

        public IntPtr getHandle()
        {
            return handle;
        }

        public void injectDll(string libraryPath)
        {
            IntPtr pathPointer = WINAPI.VirtualAllocEx(handle, IntPtr.Zero, (IntPtr)libraryPath.Length, WINAPI.AllocationType.Reserve | WINAPI.AllocationType.Commit, WINAPI.MemoryProtection.PAGE_EXECUTE_READ_WRITE);
            if (pathPointer == null) throw new VirtualAllocationException("Allocation at an undefinded location for " + libraryPath.Length + " Bytes  with  allocationType " + (WINAPI.AllocationType.Reserve | WINAPI.AllocationType.Commit) + " and memoryProtection " + WINAPI.MemoryProtection.PAGE_EXECUTE_READ_WRITE + " has failed with the errorcode " + Marshal.GetLastWin32Error());
            long bytesWritten = 0;
            if (WINAPI.WriteProcessMemory(handle, pathPointer, Encoding.ASCII.GetBytes(libraryPath), Encoding.ASCII.GetBytes(libraryPath).Length, ref bytesWritten) == false) throw new WriteProcessMemoryException("Writing "+libraryPath.Length+" Bytes to "+new Address(pathPointer)+" has failed with the errorcode "+Marshal.GetLastWin32Error());
            IntPtr kernelDllPointer = WINAPI.GetModuleHandle("Kernel32.dll");
            IntPtr loadLibraryPointer = WINAPI.GetProcAddress(kernelDllPointer, "LoadLibraryA");
            IntPtr threadHandle = WINAPI.CreateRemoteThread(handle, IntPtr.Zero, 0, loadLibraryPointer, pathPointer, 0, IntPtr.Zero);
            if (WINAPI.CloseHandle(threadHandle) == false) throw new CloseHandleException("Closing the handle " + threadHandle + " has failed with the errorcode " + Marshal.GetLastWin32Error());
        }


        public MemoryRegion allocateMemory(int size, WINAPI.AllocationType allocationType = WINAPI.AllocationType.Reserve | WINAPI.AllocationType.Commit, WINAPI.MemoryProtection memoryProtection = WINAPI.MemoryProtection.PAGE_EXECUTE_READ_WRITE)
        {
            IntPtr allocation = WINAPI.VirtualAllocEx(handle, IntPtr.Zero, (IntPtr)size, allocationType, memoryProtection);
            if (allocation == null) throw new Exception("Allocation at an undefinded location for " + size + " Bytes  with  allocationType " + allocationType + " and memoryProtection " + memoryProtection + " has failed with the errorcode " + Marshal.GetLastWin32Error());
            MemoryRegion res = new MemoryRegion();
            res.start = new Address(allocation);
            res.lenght = size;
            return res;
        }

        public MemoryRegion allocateMemoryAt(Address adr, int size, WINAPI.AllocationType allocationType = WINAPI.AllocationType.Reserve | WINAPI.AllocationType.Commit, WINAPI.MemoryProtection memoryProtection = WINAPI.MemoryProtection.PAGE_EXECUTE_READ_WRITE)
        {
            IntPtr allocation = WINAPI.VirtualAllocEx(handle, adr.getAsPointer(), (IntPtr)size, allocationType, memoryProtection);
            if (allocation == null) throw new Exception("Allocation at " + adr + " location for " + size + " Bytes  with  allocationType " + allocationType + " and memoryProtection " + memoryProtection + " has failed with the errorcode " + Marshal.GetLastWin32Error());
            MemoryRegion res = new MemoryRegion();
            res.start = new Address(allocation);
            res.lenght = size;
            return res;
        }

        private IntPtr api_OpenProcess(int pid)
        {
            IntPtr processHandle = WINAPI.OpenProcess(WINAPI.ProcessAccessFlags.All, false, pid);
            if (processHandle == null) throw new OpenProcessException("Obtaining a handle with Allaccess to the process with the pid " + pid + " has failed with errorcode " + Marshal.GetLastWin32Error());
            return processHandle;
        }

        private byte[] api_ReadProcessMemory(Address adr, int size)
        {
            uint oldProtection;
            if (WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), size, 0x40, out oldProtection) == false) throw new VirtualProtectionException("Changing the protection of "+adr+" to 0x40 has failed with the errorcode "+Marshal.GetLastWin32Error());
            byte[] array = new byte[size];
            long numRead = 0;
            if (WINAPI.ReadProcessMemory(handle, adr.getAsPointer(), array, size, ref numRead) == false) throw new ReadProcessMemoryException("Reading "+size+" Bytes from "+adr+" has failed with the errorcode"+Marshal.GetLastWin32Error());
            if (WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), size, oldProtection, out oldProtection) == false) throw new VirtualProtectionException("Reverting the protection of "+adr+" back to its previous protection has failed with the errorcode"+Marshal.GetLastWin32Error());
            return array;
        }

        private void api_WriteProcessMemory(Address adr, byte[] val)
        {
            uint oldProtection;
            if (WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), val.Length, 0x40, out oldProtection) == false) throw new VirtualProtectionException("Changing the protection of " + adr + " to 0x40 has failed with the errorcode " + Marshal.GetLastWin32Error());
            long numWritten = 0;
            if (WINAPI.WriteProcessMemory(handle, adr.getAsPointer(), val, val.Length, ref numWritten) == false) throw new WriteProcessMemoryException("Writing "+val.Length+" Bytes to "+adr+" has failed with the errorcode"+Marshal.GetLastWin32Error());
            if (WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), val.Length, oldProtection, out oldProtection) == false) throw new VirtualProtectionException("Reverting the protection of " + adr + " back to its previous protection has failed with the errorcode" + Marshal.GetLastWin32Error());
        }

        public Address getModuleBase(string name)
        {
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                if (pm.ModuleName == name) return new Address(pm.BaseAddress);
            }
            return null;
        }

        public ArrayList getLoadedModuleNames()
        {
            ArrayList res = new ArrayList();
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                res.Add(pm.ModuleName);
            }
            return res;
        }

        public ProcessModuleCollection getLoadedModules()
        {
            return managedProcess.Modules;
        }

        public int getPid()
        {
            return managedProcess.Id;
        }

        public void WriteInt32(Address adr, int val)
        {
            api_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteInt64(Address adr, long val)
        {
            api_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteFloat(Address adr, float val)
        {
            api_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteDouble(Address adr, double val)
        {
            api_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteByte(Address adr, byte val)
        {
            byte[] buf = { val };
            api_WriteProcessMemory(adr, buf);
        }

        public void WriteByteArray(Address adr, byte[] val)
        {
            api_WriteProcessMemory(adr, val);
        }

        public int ReadInt32(Address adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, sizeof(int));
            return BitConverter.ToInt32(buffer, 0);
        }

        public long ReadInt64(Address adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, sizeof(long));
            return BitConverter.ToInt64(buffer, 0);
        }

        public float ReadFloat(Address adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, sizeof(float));
            return BitConverter.ToSingle(buffer, 0);
        }

        public double ReadDouble(Address adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, sizeof(double));
            return BitConverter.ToDouble(buffer, 0);
        }

        public byte ReadByte(Address adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, 1);
            return buffer[0];
        }

        public byte[] ReadByteArray(Address adr, int size)
        {
            byte[] buffer = api_ReadProcessMemory(adr, size);
            return buffer;
        }

        public void freezeProcess()
        {
            foreach (ProcessThread pt in managedProcess.Threads)
            {
                IntPtr cHandle = IntPtr.Zero;
                cHandle = WINAPI.OpenThread(WINAPI.ThreadAccessFlags.THREAD_SUSPEND_RESUME, false, (uint)pt.Id);
                if (cHandle == null) throw new OpenThreadException("Obtaining a handle with Allaccess to the thread with the ID: " + pt.Id + " has failed with errorcode " + Marshal.GetLastWin32Error());
                WINAPI.SuspendThread(cHandle);
            }
        }

        public void unfreezeProcess()
        {
            foreach (ProcessThread pt in managedProcess.Threads)
            {
                IntPtr cHandle = IntPtr.Zero;
                cHandle = WINAPI.OpenThread(WINAPI.ThreadAccessFlags.THREAD_SUSPEND_RESUME, false, (uint)pt.Id);
                if (cHandle == null) throw new OpenThreadException("Obtaining a handle with Allaccess to the thread with the ID: " + pt.Id + " has failed with errorcode " + Marshal.GetLastWin32Error());
                WINAPI.ResumeThread(cHandle);
            }
        }

        public void terminate()
        {
            managedProcess.Kill();
        }
    }
}
