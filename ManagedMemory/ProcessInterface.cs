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
    class ProcessInterface
    {
        private Process managedProcess;
        private IntPtr handle;

        public ProcessInterface(string name)
        {
            Process[] procs = Process.GetProcessesByName(name);
            if (procs.Length != 1) throw new Exception("Process is not unique or does not exist");
            handle = api_OpenProcess(managedProcess.Id);

        }

        private IntPtr api_OpenProcess(int pid)
        {
            return WINAPI.OpenProcess(WINAPI.ProcessAccessFlags.All, false, pid);
        }

        private byte[] api_ReadProcessMemory(Address64 adr, int size)
        {
            uint flNewProtect;
            WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), size, 0x40, out flNewProtect); //4u 0x40=PAGE_EXECUTE_RW
            byte[] array = new byte[size];
            long numRead = 0;
            WINAPI.ReadProcessMemory(handle, adr.getAsPointer(), array, size, ref numRead);
            WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), size, flNewProtect, out flNewProtect);
            return array;
        }

        private void api_WriteProcessMemory(Address64 adr, byte[] val)
        {
            uint flNewProtect;
            WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), val.Length, 0x40, out flNewProtect);
            long numWritten = 0;
            bool flag = WINAPI.WriteProcessMemory(handle, adr.getAsPointer(), val, val.Length, ref numWritten);
            WINAPI.VirtualProtectEx(handle, adr.getAsPointer(), val.Length, flNewProtect, out flNewProtect);
        }

        public Address64 getModuleBase(string name)
        {
            foreach(ProcessModule pm in managedProcess.Modules)
            {
                if (pm.ModuleName == name) return new Address64(pm.BaseAddress);
            }
            return null;
        }

        public ArrayList getLoadedModuleNames()
        {
            ArrayList res = new ArrayList();
            foreach(ProcessModule pm in managedProcess.Modules)
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

        public void WriteInt32(Address64 adr,int val)
        {
            api_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteInt64(Address64 adr, long val)
        {
            api_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteFloat(Address64 adr, float val)
        {
            api_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteDouble(Address64 adr, double val)
        {
            api_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteByte(Address64 adr, byte val)
        {
            byte[] buf = { val };
            api_WriteProcessMemory(adr, buf);
        }

        public void WriteByteArray(Address64 adr, byte[] val)
        {
            api_WriteProcessMemory(adr, val);
        }

        public int ReadInt32(Address64 adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, sizeof(int));
            return BitConverter.ToInt32(buffer, 0);
        }

        public long ReadInt64(Address64 adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, sizeof(long));
            return BitConverter.ToInt64(buffer, 0);
        }

        public float ReadFloat(Address64 adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, sizeof(float));
            return BitConverter.ToSingle(buffer, 0);
        }

        public double ReadDouble(Address64 adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, sizeof(double));
            return BitConverter.ToDouble(buffer, 0);
        }

        public byte ReadByte(Address64 adr)
        {
            byte[] buffer = api_ReadProcessMemory(adr, 1);
            return buffer[0];
        }

        public byte[] ReadByteArray(Address64 adr, int size)
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
