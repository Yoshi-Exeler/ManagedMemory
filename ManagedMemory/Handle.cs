using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
