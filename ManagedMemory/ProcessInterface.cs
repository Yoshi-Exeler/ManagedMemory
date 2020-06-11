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

        //Searches the process for the specified pattern, then returns the address of the first byte of the pattern offset by the FinalOffset
        // or null if the pattern was not found. This method is resource intensive as it creates a full memory dump of the target process.
        public Address findPattern(string module, byte[] pattern, string mask, int finalOffset)
        {
            ProcessModule mod = null;
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                if (pm.ModuleName == module) mod = pm;
            }
            if (mod == null) throw new Exception("specified module not found");
            byte[] dump = new byte[mod.ModuleMemorySize];
            long numRead = 0;
            WINAPI.ReadProcessMemory(handle, mod.BaseAddress, dump, dump.Length, ref numRead);
            bool found = false;
            long resIndex = -1;
            for (int i = 0; i < dump.Length; i++)
            {
                for (int x = 0; x < pattern.Length; x++)
                {
                    if (i + x > dump.Length) break;
                    if (!(mask[x] == '?' || dump[i + x] == pattern[x])) break;
                    if (x == pattern.Length) found = true;
                    resIndex = i;
                }
                if (found) break;
            }
            if (!found) return null;
            return new Address((long)mod.BaseAddress + resIndex + finalOffset);
        }

        private IntPtr api_OpenProcess(int pid)
        {
            return WINAPI.OpenProcess(WINAPI.ProcessAccessFlags.All, false, pid);
        }

        private byte[] api_ReadProcessMemory(Address adr, int size)
        {
            uint oldProtection;
            WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), size, 0x40, out oldProtection); //4u 0x40=PAGE_EXECUTE_RW
            byte[] array = new byte[size];
            long numRead = 0;
            WINAPI.ReadProcessMemory(handle, adr.getAsPointer(), array, size, ref numRead);
            WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), size, oldProtection, out oldProtection);
            return array;
        }

        private void api_WriteProcessMemory(Address adr, byte[] val)
        {
            uint oldProtection;
            WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), val.Length, 0x40, out oldProtection);
            long numWritten = 0;
            bool flag = WINAPI.WriteProcessMemory(handle, adr.getAsPointer(), val, val.Length, ref numWritten);
            WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), val.Length, oldProtection, out oldProtection);
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
                cHandle = WINAPI.OpenThread(WINAPI.ProcessAccessFlags.All, false, (uint)pt.Id);
                WINAPI.SuspendThread(cHandle);
            }
        }

        public void unfreezeProcess()
        {
            foreach (ProcessThread pt in managedProcess.Threads)
            {
                IntPtr cHandle = IntPtr.Zero;
                cHandle = WINAPI.OpenThread(WINAPI.ProcessAccessFlags.All, false, (uint)pt.Id);
                WINAPI.ResumeThread(cHandle);
            }
        }

        public void terminate()
        {
            managedProcess.Kill();
        }
    }
}
