using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ManagedMemory
{
    public class Handle
    {
        protected long handle;

        public Handle(long handle)
        {
            this.handle = handle;
        }

        public Handle(IntPtr handle)
        {
            this.handle = (long)handle;
        }

        public static Handle GetProcessHandle(string name, APIProxy.ProcessAccessFlags access)
        {
            Process[] procs = Process.GetProcessesByName(name);
            if (procs.Length != 1) throw new Exception("process is not unique or does not exist");
            return APIProxy.OpenProcess(access, procs[0].Id);
        }

        public static Handle GetThreadHandle(uint threadID, APIProxy.ThreadAccessFlags access)
        {
            return APIProxy.OpenThread(access, threadID);
        }

        public static Handle GetModuleHandle(string moduleName)
        {
            return APIProxy.GetModuleHandle(moduleName);

        }

        public long GetHandleAsLong()
        {
            return handle;
        }

        public IntPtr GetHandleAsPointer()
        {
            return (IntPtr)handle;
        }

        public void Close()
        {
            APIProxy.CloseHandle(this);
        }

    }
}
