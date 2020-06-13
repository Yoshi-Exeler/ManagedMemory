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
        protected ProcessInterface callback;

        public Handle(long handle, ProcessInterface callback)
        {
            this.handle = handle;
            this.callback = callback;
        }

        public Handle(IntPtr handle, ProcessInterface callback)
        {
            this.handle = (long)handle;
            this.callback = callback;
        }

        public static Handle GetProcessHandle(string name,APIProxy.ProcessAccessFlags access, ProcessInterface callback)
        {
            Process[] procs = Process.GetProcessesByName(name);
            if (procs.Length != 1) throw new Exception("process is not unique or does not exist");
            return new Handle(APIProxy.OpenProcess(access, procs[0].Id),callback);
        }

        public static Handle GetThreadHandle(uint threadID,APIProxy.ThreadAccessFlags access,ProcessInterface callback)
        {
            return new Handle(APIProxy.OpenThread(access, threadID), callback);
        }

        public static Handle GetModuleHandle(string moduleName, ProcessInterface callback)
        {
            return new Handle(APIProxy.GetModuleHandle(moduleName), callback);

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
            APIProxy.CloseHandle((IntPtr)handle);
        }

    }
}
