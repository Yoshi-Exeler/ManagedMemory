using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
            handle = api_openprocess(managedProcess.Id);
            
        }

        private IntPtr api_OpenProcess(int pid)
        {
            return WINAPI.OpenProcess(WINAPI.ProcessAccessFlags.All, false, pid);
        }

        private byte[] api_ReadProcessMemory(int adr,int size)
        {
            uint flNewProtect;
            WINAPI.VirtualProtectEx(handle, (IntPtr)adr, size, 0x40, out flNewProtect); //4u 0x40=PAGE_EXECUTE_RW
            byte[] array = new byte[size];
            long numRead = 0;
            WINAPI.ReadProcessMemory(handle, (IntPtr)adr, array, size, ref numRead);
            WINAPI.VirtualProtectEx(handle, (IntPtr)adr, size, flNewProtect, out flNewProtect);
            return array;
        }

        private void api_WriteProcessMemory(int adr,byte[] val)
        {
            uint flNewProtect;
            WINAPI.VirtualProtectEx(handle, (IntPtr)adr, val.Length, 0x40, out flNewProtect);
            long numWritten = 0;
            bool flag = WINAPI.WriteProcessMemory(handle, (IntPtr)adr, val, val.Length, ref numWritten);
            WINAPI.VirtualProtectEx(handle, (IntPtr)adr, val.Length, flNewProtect, out flNewProtect);
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
