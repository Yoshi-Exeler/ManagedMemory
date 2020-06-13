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

        public static Handle Zero()
        {
            return new Handle(IntPtr.Zero);
        }

        public Handle(IntPtr handle)
        {
            this.handle = (long)handle;
        }

        public static Handle GetProcessHandle(string name, APIProxy.ProcessAccessFlags access)
        {
            Process proc = ProcessInterface.FindProcess(name);
            return APIProxy.OpenProcess(access, proc.Id);
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

        public override string ToString()
        {
            return "0x"+Convert.ToString(handle, 16);
        }

    }
}
